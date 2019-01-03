// Copyright 2004, 2011 hyam <hyamhyam@gmail.com>
// and it is under GPL
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2, or (at your option)
// any later version.
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.IO;
using System.Reflection;
using System.Globalization ;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;
using System.Runtime.InteropServices; 
using Microsoft.VisualBasic;

using HYAM.Lingua;

namespace HYAM.KJ_dict
{

    public partial class KJ_form : System.Windows.Forms.Form
    {
        // 1個前の検索見出し語
        string prev_keyword;

#if EDICT 
        // 1個前の辞書引きの答え
        string prev_answer;
#endif
        
        //--------------------------------------------------------------------
        // 翻訳処理
        public void form_search()
        {
            
            if(translatorThread != null){
                //  検索中を示す背景色とタイトルを戻す 
                this.BackColor = System.Drawing.SystemColors.Control ;
#if !EDICT 
                this.Text = "KJ_dictform";
#else
                this.Text = "KJ_Edict";
#endif
                
                translatorThread.Abort();
                // filterを戻す
                KJ_dict.SetFilter(this.filter);
            }
            
            // 現在のfilterをbackup
            this.filter = KJ_dict.GetFilter();
            
            // 処理用のスレッドを作成
            translatorThread = new Thread(new ThreadStart(this.form_search_thread));
            translatorThread.Start();

            // filterを戻す
            KJ_dict.SetFilter(this.filter);
            
        }

        //--------------------------------------------------------------------
        // Windows フォーム コントロールのスレッド セーフな呼び出し の対策
        //  http://msdn.microsoft.com/ja-jp/library/ms171728(VS.80).aspx
        delegate void SetTextCallback(string text);
// for RichTextBoxEx        
//        delegate void AddTextCallback(string text);
//        delegate void AddlinkCallback(string text);
        
        private void SetText(string text)
        {
            if (this.richTextBox2.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.richTextBox2.Text = text;
            }
        }
// for RichTextBoxEx
//        private void AddText(string text)
//        {
//            if (this.richTextBox2.InvokeRequired)
//            {
//                AddTextCallback d = new AddTextCallback(AddText);
//                this.Invoke(d, new object[] { text });
//            }
//            else
//            {
//                this.richTextBox2.SelectedText = text;
//               // this.richTextBox2.Text += text;
//            }
//        }
//        private void AddLink(string text)
//        {
//            if (this.richTextBox2.InvokeRequired)
//            {
//                AddlinkCallback d = new AddlinkCallback(AddLink);
//                this.Invoke(d, new object[] { text });
//            }
//            else
//            {
//                this.richTextBox2.InsertLink(text);
//            }
//        }

        //--------------------------------------------------------------------
        // 検索用スレッド
        //   (正規表現検索に入ると時間がかかる場合があるため別スレッドにする)
        public void form_search_thread()
        {
            Cursor preCursor = Cursor.Current;

            //  検索中を示すためタイトルを変える 
            this.Text = this.searchingLabel ;

            String searchword = this.inputArea.Text ;
            
            
            searchword = searchword.Trim();
            if(searchword == ""){
                richTextBox2.Text = "";

                //  検索中を示すタイトルを戻す 
#if !EDICT 
                this.Text = "KJ_dictform";
#else
                this.Text = "KJ_Edict";
#endif
                // 検索語がない
                return ;
            }
            
#if EDICT 
            // 大文字→小文字変換する。
            //   There had to be times... のように
            //   文頭の大文字始まりの単語を引けるように。
            searchword = searchword.ToLower();
#endif
            
            // 検索の種別
            SearchType  searchType = SearchType.forward ; //default    
            
            if(this.radioButton1.Checked == true)
            {
                searchType = SearchType.full ;      // 完全一致
            }
            if(this.radioButton2.Checked == true)
            {
                searchType = SearchType.forward ;   // 前方一致
            }
            if(this.radioButton3.Checked == true)
            {
                searchType = SearchType.backward ;  // 後方一致
            }
            if(this.radioButton4.Checked == true)
            {
                searchType = SearchType.part ;      // 部分一致
            }
            
            
            SearchResult result;
            
            if(searchword.IndexOf("*") < 0  &&
               searchword.IndexOf("+") < 0  &&
               searchword.IndexOf(".") < 0  &&
               searchword.IndexOf("^") < 0  &&
               searchword.IndexOf("$") < 0  &&
               searchword.IndexOf("(") < 0  &&
               searchword.IndexOf("[") < 0  &&
               searchword.IndexOf("{") < 0  &&
               searchword.IndexOf("?") < 0      ){
                // 通常検索
                result = KJ_dict.DictSearch( searchType , searchword );
            }else{
                // 正規表現検索
                result = RegexSearch(searchword );
            }
            
            // make result text
       //     MakeResultText(searchword, searchType, result); // for RichTextBoxEx
            this.SetText( MakeResultText(searchword, searchType, result) );
            
            //  検索中を示す背景色とタイトルを戻す 
            this.BackColor = System.Drawing.SystemColors.Control ;
#if !EDICT 
            this.Text = "KJ_dictform";
#else
            this.Text = "KJ_Edict";
#endif
        }
        
        //----------------------------------------------------------------------
        private SearchResult RegexSearch(string searchword )
        {

            // 正規表現の形式誤りをチェックする
            try{
                Regex regex = new Regex(searchword);
            }catch{
                SearchResult regexerr = new SearchResult();
                regexerr.return_code = -20 ;
                return regexerr;
            }
            
            // サポート外の正規表現
            if(searchword.IndexOf("(")>=0  ||
               searchword.IndexOf(")")>=0  ||
               searchword.IndexOf("^")>=0  ||
               searchword.IndexOf("$")>=0       ) {   
                SearchResult notfound = new SearchResult();
                notfound.return_code = -21 ;
                return notfound;
            }
            
            // 正規表現検索
            SearchType searchType = SearchType.regex ;
            return  KJ_dict.RegexSearch(searchType, searchword );
            
        }
        
        //----------------------------------------------------------------------
        private bool InputTextIsHangul(string searchword )
        {
        //  if( CodeCheck.IsAlpah(searchword) || CodeCheck.IsHangul(searchword) ){
        //        return true;   
        //    }else{
        //        return false;  
        //    }

        // OSの言語依存にする(2008.12.30)
            // 入力がハングルなら InputTextIsHangul true
            // 入力が漢字・ひらがな・カタカナならfalse
            //
            // 入力が数字・英語の場合は，OSに依存。
            //    ・OSが"ja-JP"ならtrue(ハングル扱い)
            //    ・OSが"ja-JP"以外ならfalse
            if( CodeCheck.IsHangul(searchword) ){
                return true;   
            }
            
            if(this.cultureName == "ja-JP"){
                if( CodeCheck.IsAlpah(searchword) || 
                    CodeCheck.IsDigit(searchword)
                ){
                    return true;   
                }
                
                if( CodeCheck.IsHiragana(searchword) || 
                    CodeCheck.IsKanji(searchword)    ||
                    CodeCheck.IsKatakana(searchword) 
                ){
                    return false;   
                }
                
            }else{
                if( CodeCheck.IsAlpah(searchword) || 
                    CodeCheck.IsDigit(searchword)
                ){
                    return false;   
                }
                if( CodeCheck.IsHiragana(searchword) || 
                    CodeCheck.IsKanji(searchword)    ||
                    CodeCheck.IsKatakana(searchword) 
                ){
                    return false;   
                }
                
            }
            return true;   
        }


        //----------------------------------------------------------------------
        // 検索結果の構造体を表示用のテキストにする
//        public void MakeResultText(string searchword, // for RichTextBoxEx
        public string MakeResultText(string searchword, 
                                     SearchType searchType, 
                                     SearchResult result)
        {
            
            if(result == null){
//                return ;
                return "";
            }
            
            if(result.return_code == -1){  
                // 見つかりません 
                
// for RichTextBoxEx
             //   this.SetText(msg.Get("E003") + " \"" + 
             //                this.inputArea.Text + "\"\n");
             //   return;
                return(msg.Get("E003") + " \"" + this.inputArea.Text + "\"\n");
            }
            
            if(result.return_code == -20){  
                // 正規表現誤り 
//                this.SetText(msg.Get("E005") + " \"" + searchword + "\"\n");
//                return;
                return (msg.Get("E005") + " \"" + searchword + "\"\n");
            }
            if(result.return_code == -21){  
                // 正規表現誤り 
//                this.SetText(msg.Get("E006") + " \"" + searchword + "\"\n");
//                return;
                return (msg.Get("E006") + " \"" + searchword + "\"\n");
            }

            this.inputIsHangul = InputTextIsHangul(searchword);
            
            StringBuilder rtn_str = new StringBuilder();
            rtn_str.Length = 0;
            

            prev_keyword = "";
            
            // 検索した語の数だけループ
            foreach ( DocumentData sd  in  result.documents  ){
                if(sd==null){
                    continue;
                }
                rtn_str.Append( DisplayResult(searchword , sd) ) ; 
            }        
            
            if(result.return_code == 1){  // over 100 counts
                //  検索結果が100個を超えました。検索条件を見直して下さい。
//                this.AddText(  msg.Get("E001") + "\n" );
                rtn_str.Append( "\n\n" +  msg.Get("E001") + "\n" );
            }
            
// for RichTextBoxEx
//            return ;
            return rtn_str.ToString();
            
        }


#if !EDICT             
        //----------------------------------------------------------------------
        // 1つの語のデータの表示 (通常のKJ_dict用)
        //
        private string DisplayResult(string searchword , DocumentData sd){  
                                    
            StringBuilder rtn_str = new StringBuilder();
            rtn_str.Length = 0;
            
            string key_from="";    // 検索語
            string key_to="";
            string word_from = "";
            string word_to = "";
            string detail="";
            string ex="";
            string cost="";
            string src="";
            string pos="";           // 品詞情報、格変化 (暫定)

            String indent = "";
            
            // 入力がハングルならfromがkey1(ハングル)
            // 入力が漢字・ひらがな・カタカナならfromがkey2(日本語)
            //
            // 入力が数字・英語の場合は，OSに依存。
            //    ・OSが"ja-JP"ならfromがkey1(ハングル)
            //    ・OSが"ja-JP"以外ならfromがkey2(日本語)
            if(this.inputIsHangul){
                key_from  = sd.GetData("key1"); 
                key_to    = sd.GetData("key2");
                word_from = sd.GetData("word1");
                word_to   = sd.GetData("word2");
                ex        = sd.GetData("ex1");
                cost      = sd.GetData("cost2");
            }else{
                key_from  = sd.GetData("key2");   // Jp
                key_to    = sd.GetData("key1");   // Kr
                word_from = sd.GetData("word2");
                word_to   = sd.GetData("word1");
                ex        = sd.GetData("ex2");
                cost      = sd.GetData("cost1");
            }
       

            // 表示の見た目を整える。インデントつけたり、括弧をつけたり。
            
            // 入力語の表示
            
            rtn_str.Append(key_from); 
            if(word_from != ""){
                // 詳細情報は以下の場合だけ
                // ・韓国OSのとき 
                //   または
                // ・デバッグ情報表示時    
                if(this.cultureName == "ko-KR"  || 
                   CodeCheck.IsKanji(word_from) ||
                   Setting.debugInfo   ) {
                    // 詳細情報。ハングルの旧漢字、日本語の読み
                    rtn_str.Append("  〔 " + word_from + " 〕" ); 
                }
            }
            rtn_str.Append("\n");

            
            // K-->Jの時の入力ハングルのカナ表記の表示
            if(this.Setting.withPronunciation) {
                if(this.inputIsHangul){
                    if(this.Setting.PronunciationType == 1){
                        string  kana = Hangul.Hangul2Kana(key_from);
                        rtn_str.Append(" (" + kana  + ")\n" ); 
                    }else{
                        rtn_str.Append(indent + " (" + 
                                       sd.GetData("pronun1")  + ")\n" ); 
                    }
                }
            }

            
            // もしあれば品詞情報
            pos = MakePosString(sd);
            if(pos != null && pos != ""){
                rtn_str.Append(indent + "【 " + pos + " 】\n"); 
            }

            // もしあれば原形表示
            string root="";
            if(inputIsHangul){
                root = sd.GetData("root1");        
            }else{
                root = sd.GetData("root2");
            }
            if(root != ""){
                string rootname = Pos.conjugationName("conjugation_root");
                rtn_str.Append(indent + "〔(" + rootname + ") " 
                                              + root + "〕\n" ); 
            }

            // 結果の表示
            rtn_str.Append(indent + key_to ); 
  

            // 詳細情報は以下の場合だけ
            // ・韓国OSのとき 
            //   または
            // ・デバッグ情報表示時    
            if(word_to != ""){
                if(this.cultureName == "ko-KR" || 
                   CodeCheck.IsKanji(word_to)  ||
                   Setting.debugInfo    ){
                    rtn_str.Append(" 〔 " + word_to  + " 〕" ); 
                    // 詳細情報。ハングルの旧漢字、日本語の読み
                }
            }
            rtn_str.Append("\n");

            // J-->Kの時の結果ハングルのカナ表記の表示
            if(this.Setting.withPronunciation) {
                // 表示の設定がされているときだけ
                if(!this.inputIsHangul){
                    if(this.Setting.PronunciationType == 1){
                        string  kana = Hangul.Hangul2Kana(key_to);
                        rtn_str.Append(indent + " (" + kana  + ")\n" ); 
                    }else{
                        rtn_str.Append(indent + " (" + 
                            sd.GetData("pronun1")  + ")\n" ); 
                    }
                }
            }
            
            // その他付加情報
            detail = MakeDetailString(sd);
            if(detail != null && detail != ""){
                rtn_str.Append(indent + "( " + detail + " )\n"); 
            }
            if(ex != ""){
                rtn_str.Append(indent + "Ex. "+  ex + "\n"); 
            }

            
            // 「デバッグ情報表示」 を選んだ場合
            if(Setting.debugInfo){
                src = sd.GetData("src");
                if(src != ""){
                    rtn_str.Append(indent + "src:" + src + "\n"); 
                }
                if(cost != ""){
                    rtn_str.Append(indent + "cost:" + cost + "\n"); 
                }
                string pos2=sd.GetPos(); // posの生データ
                if(pos2 != ""){
                    rtn_str.Append(indent + "pos:" + pos2 + "\n"); 
                }
            }
                
            rtn_str.Append("\n");
            
            prev_keyword = key_from;   // 記憶
            
            return rtn_str.ToString();
        }
#else
        //------------------------------------------------------------------
        // 1つの語のデータを，表示用のstringに整形する。(edict用)
        //
        private string DisplayResult(string searchword , DocumentData sd){  
            StringBuilder rtn_str = new StringBuilder();
            rtn_str.Length = 0;
            
            string key_from="";    // 検索語
            string key_to="";
            string word_from = "";
            string word_to = "";
       //     string pos="";           // 品詞情報、格変化 (暫定)
       //     String indent = "";

            
            // 入力がアルファベットならfromがkey1(英語)
            // 入力が漢字・ひらがな・カタカナならfromがkey2(日本語)
            //
            // 入力が数字・英語の場合は，OSに依存。
            //    ・OSが"ja-JP"ならfromがkey1(英語)
            //    ・OSが"ja-JP"以外ならfromがkey2(日本語)
            if(this.inputIsHangul){
                key_from  = sd.GetData("key1"); 
                key_to    = sd.GetData("key2");
                word_from = sd.GetData("word1");
                word_to   = sd.GetData("word2");
            }else{
                key_from  = sd.GetData("key2");   // Jp
                key_to    = sd.GetData("key1");   // Kr
                word_from = sd.GetData("word2");
                word_to   = sd.GetData("word1");
            }
       

            // 表示の見た目を整える。インデントつけたり、括弧をつけたり。

            if(prev_keyword != key_from && prev_keyword != ""){
                rtn_str.Append("\n\n");
            }
            
            // 入力語の表示
            if(prev_keyword != key_from){
                rtn_str.Append(key_from); 
                rtn_str.Append("\n");
            }else{
                // 見出し語が一緒なら結果を ","で繋いで束ねる。
                if(key_to != prev_answer){
                    rtn_str.Append(",");
                }
            }

            // 結果の表示
            if(prev_keyword == key_from && key_to == prev_answer){
                // NOP   重複する結果は表示しない
            }else{
                rtn_str.Append(key_to); 
            }  
            
            // もしあれば品詞情報 [2011/01/15 19:18:52]
     //edictの品詞情報は日本語についているので，
     //表示してもあまり意味がない。そのため表示しない。
     //       pos = MakePosString(sd);
     //       if(pos != null && pos != ""){
     //           rtn_str.Append(indent + "【 " + pos + " 】\n"); 
     //       }
        // ～2011.01.15(old type)
        //    // もしあれば品詞情報
        //    string pos = sd.GetData("pos");
        //    if(pos != null && pos != ""){
        //        rtn_str.Append("\n" + pos); 
        //    }
            
            
            // もしあれば補足情報
            string detail1 = sd.GetData("detail1");
            if(detail1 != null && detail1 != ""){
                rtn_str.Append("\n" + detail1); 
            }

            prev_keyword = key_from;   // 記憶
            prev_answer  = key_to;
            
            return rtn_str.ToString();
            
        } // end of DisplayResult_edict
#endif
        
//        //----------------------------------------------------------------------
//        // 1つの語のデータを，表示用のstringに整形する。(for RichTextBoxEx)
//        //
//        //  結果表示の中からリンククリックによる再検索などを実装する。
//        //  （作成途中のため，コメント化。[2010/07/19 16:59:17]）
//        //
//        private void DisplayResult_Ex(string searchword , DocumentData sd){  
//                                    
//            StringBuilder rtn_str = new StringBuilder();
//            rtn_str.Length = 0;
//            
//            string key_from="";    // 検索語
//            string key_to="";
//            string word_from = "";
//            string word_to = "";
//            string detail="";
//            string ex="";
//            string cost="";
//            string src="";
//            string pos="";           // 品詞情報、格変化 (暫定)
//
//            String indent = "";
//            
//            // 入力がハングルならfromがkey1(ハングル)
//            // 入力が漢字・ひらがな・カタカナならfromがkey2(日本語)
//            //
//            // 入力が数字・英語の場合は，OSに依存。
//            //    ・OSが"ja-JP"ならfromがkey1(ハングル)
//            //    ・OSが"ja-JP"以外ならfromがkey2(日本語)
//            if(this.inputIsHangul){
//                key_from  = sd.GetData("key1"); 
//                key_to    = sd.GetData("key2");
//                word_from = sd.GetData("word1");
//                word_to   = sd.GetData("word2");
//                ex        = sd.GetData("ex1");
//                cost      = sd.GetData("cost2");
//            }else{
//                key_from  = sd.GetData("key2");   // Jp
//                key_to    = sd.GetData("key1");   // Kr
//                word_from = sd.GetData("word2");
//                word_to   = sd.GetData("word1");
//                ex        = sd.GetData("ex2");
//                cost      = sd.GetData("cost1");
//            }
//       
//
//            // 表示の見た目を整える。インデントつけたり、括弧をつけたり。
//            
//            // 入力語の表示
//            
//            this.AddLink(key_from); 
//            if(word_from != ""){
//                // 詳細情報は以下の場合だけ
//                // ・韓国OSのとき 
//                //   または
//                // ・デバッグ情報表示時    
//                if(this.cultureName == "ko-KR"  || 
//                   CodeCheck.IsKanji(word_from) ||
//                   Setting.debugInfo   ) {
//                    // 詳細情報。ハングルの旧漢字、日本語の読み
//                    this.AddText("  〔 " + word_from + " 〕" ); 
//                }
//            }
//            this.AddText("\n");
//
//            
//            // K-->Jの時の入力ハングルのカナ表記の表示
//            if(this.Setting.withPronunciation) {
//                if(this.inputIsHangul){
//                    if(this.Setting.PronunciationType == 1){
//                        string  kana = Hangul.Hangul2Kana(key_from);
//                        this.AddText(" (" + kana  + ")\n" ); 
//                    }else{
//                        this.AddText(indent + " (" + 
//                                       sd.GetData("pronun1")  + ")\n" ); 
//                    }
//                }
//            }
//
//            
//            // もしあれば品詞情報
//            pos = MakePosString(sd);
//            if(pos != null && pos != ""){
//                this.AddText(indent + "【 " + pos + " 】\n"); 
//            }
//
//            // 結果の表示
//            this.AddText(indent + key_to ); 
//  
//
//            // 詳細情報は以下の場合だけ
//            // ・韓国OSのとき 
//            //   または
//            // ・デバッグ情報表示時    
//            if(word_to != ""){
//                if(this.cultureName == "ko-KR" || 
//                   CodeCheck.IsKanji(word_to)  ||
//                   Setting.debugInfo    ){
//                    this.AddText(" 〔 " + word_to  + " 〕" ); 
//                    // 詳細情報。ハングルの旧漢字、日本語の読み
//                }
//            }
//            this.AddText("\n");
//
//            // J-->Kの時の結果ハングルのカナ表記の表示
//            if(this.Setting.withPronunciation) {
//                // 表示の設定がされているときだけ
//                if(!this.inputIsHangul){
//                    if(this.Setting.PronunciationType == 1){
//                        string  kana = Hangul.Hangul2Kana(key_to);
//                        this.AddText(indent + " (" + kana  + ")\n" ); 
//                    }else{
//                        this.AddText(indent + " (" + 
//                            sd.GetData("pronun1")  + ")\n" ); 
//                    }
//                }
//            }
//            
//            // その他付加情報
//            detail = MakeDetailString(sd);
//            if(detail != null && detail != ""){
//                this.AddText(indent + "( " + detail + " )\n"); 
//            }
//            if(ex != ""){
//                this.AddText(indent + "Ex. "+  ex + "\n"); 
//            }
//
//            
//            // 「デバッグ情報表示」 を選んだ場合
//            if(Setting.debugInfo){
//                src = sd.GetData("src");
//                if(src != ""){
//                    this.AddText(indent + "src:" + src + "\n"); 
//                }
//                if(cost != ""){
//                    this.AddText(indent + "cost:" + cost + "\n"); 
//                }
//                string pos2=sd.GetPos(); // posの生データ
//                if(pos2 != ""){
//                    this.AddText(indent + "pos:" + pos2 + "\n"); 
//                }
//            }
//                
//            this.AddText("\n");
//            
//            prev_keyword = key_from;   // 記憶
//            
//            return ;
//        }
//        

        
        //------------------------------------------------------------------
        // KJ_form デフォルトのフィルタ    costにより表示しない
        public bool defaultFilter(DocumentData doc){
            if(this.Setting.except8888 && 
               doc.GetCost(this.inputIsHangul) == "8888"){
                return false;
            }
            if(this.Setting.except9999 && 
               doc.GetCost(this.inputIsHangul) == "9999"){
                return false;
            }
            
            // 7777は無条件に非表示
            if(doc.GetCost(this.inputIsHangul) == "7777"){
                return false;
            }
            
            return true;
        }

                
        //------------------------------------------------------------
        // 表示用品詞情報作成
        private String MakePosString(DocumentData sd){  
            string pos="";
            
            // posタグの調査
            if(this.inputIsHangul){
                pos       = sd.GetData("pos1"); 
            }else{
                pos       = sd.GetData("pos2"); 
            }
            if(pos == ""){
                // pos1 or pos2がないなら posで探す
                pos = sd.GetPos();
            }
            if(pos != ""){
                // posタグを詳細表示にする
                string posName = Pos.Name(pos);
                
                if(posName != ""){

                    string conjugation="";
                    if(inputIsHangul){
                        conjugation = sd.GetData("conju1");        
                    }else{
                        conjugation = sd.GetData("conju2");
                    }
                    if(conjugation != ""){
                        posName = posName + " " + conjugation ;
                    }
                    
                    return posName ;
                }
            }
            

            return pos;
        }

        //------------------------------------------------------------
        // 表示用詳細情報の作成
        private String MakeDetailString(DocumentData sd){  
           // string pos;
            string detail="";
            
            string detailTagData;
            if(this.inputIsHangul){
                detailTagData = sd.GetData("detail2");        
            }else{
                detailTagData = sd.GetData("detail1");
            }
            
            if(detailTagData != ""){
                if(detail != ""){
                    detail = detail + " " + detailTagData;
                }else{
                    detail = detailTagData;
                }
            }

            return detail;
        }

    } // end of class KJ_form
    

}
