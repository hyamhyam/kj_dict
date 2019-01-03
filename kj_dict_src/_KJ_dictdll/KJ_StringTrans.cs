// Copyright 2004, 2009 hyam <hyamhyam@gmail.com>
// and it is under GPL
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2, or (at your option)
// any later version.

using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HYAM.Lingua;

namespace HYAM.KJ_dict
{

    //-------------------------------------------------------------------
    // 入力文字列の翻訳を行うメインクラス
    
    public class StringTrans
    {

        private bool debugInfo;          // デバッグ情報を出すか？
        private bool autoconv;           // 未知語を自動変換するか？
        private bool showoriginal;       // 原文表示するか


        private ProgressBar pBar;        // KJ_form2のProgress Bar

        private bool isBrowser  = false;   // KJ_browserからの起動か？

        
        //-------------------------------------------------------------------
        // コンストラクタ
        public StringTrans()     {
            this.pBar       = null ;
            
            // 一度ひいた辞書内容を格納するHashを使用する
            KJ_dict.HashAvailable = true; 

            // Open dictionary
            KJ_dict.DictOpen("KJ_dict.yml", "KJ_dict.yml.small");
                            // 翻訳では字母分解とかローマ字で検索しないので
                            // それらを省いた小さいarrayを使う
        }
        
        // コンストラクタ2 (KJ_form2用)
        public StringTrans( ProgressBar pBar ) : this()     {
            this.pBar = pBar ;
        }
        
        // コンストラクタ3 (プログレスバーなし， KJ_browser用:tagスキップあり)
        public StringTrans(bool isBrowser) : this()     {
            this.isBrowser = isBrowser ;
        }

        // コンストラクタ4 (プログレスバーあり， KJ_browser用:tagスキップあり)
        public StringTrans( ProgressBar pBar , bool isBrowser) : this()     {
            this.pBar      = pBar ;
            this.isBrowser = isBrowser ;
        }
        
        
        //-------------------------------------------------------------------
        // property
        public bool DebugInfo {
            //  true : デバッグ情報を表示する
            get{ return debugInfo; }
            set{ debugInfo = value; }
        }
        public bool AutoConv {
            //  true : 未知語を自動変換する
            get{ return autoconv; }
            set{ autoconv = value; }
        }
        public bool ShowOriginal {
            //  true : 原文を表示するか否か
            set{ showoriginal = value; }
        }
        
        //---------------------------------------------------------------------
        // 入力文字列の変換を試みる。
        public String Trans(String inputstring)
        {

            TextScanner scanner = new TextScanner();
            
            // ブラウザからならHTMLとしてscanさせる(タグをまとめる)
            if(isBrowser){
                scanner.htmlText = true;
            }
            
            // inputstringを語のchainに分解する
            WordChain wc = scanner.Scan(inputstring);  

            if(this.pBar != null) {
                this.pBar.Maximum = wc.Length * 2  ;
            }
            
            // 形態素解析 & 翻訳処理  （本格的じゃないよ）
            KJ_Analyzer analyzer = new KJ_Analyzer(this);
            wc = analyzer.MorphologicScan(wc);  // 翻訳はここ
            
            

            // WordChainから翻訳後文字列を作り出す    
            if( showoriginal ){
                return Chain2String_for_Web(wc); // Web用
            }else{
                return Chain2String(wc);
            }

        }
    
        
        //---------------------------------------------------------------------
        // プログレスバーを進める
        public void pBarStep()
        {
            if(this.pBar != null){
                this.pBar.PerformStep();
            }
        }
        
        //-------------------------------------------------------------------
        // WordChainの各WordTableから翻訳結果を取り出し翻訳後文字列を返す
        private String Chain2String(WordChain wc)
        {
            StringBuilder     result;       // 出力文字列
            result = new StringBuilder();
            
            
            // debuginfo is True
            if(this.debugInfo){
                result.Append( "original ⇒ result " + 
                               "(char)(pos)(GetWordCost)(divided)\n");
            }

            // 語のchainを先頭から舐め、翻訳結果をresultに順次追加。
            WordTable wt = wc.Head;

            while ( wt!=null )
            {
                // debuginfo is True
                if(this.debugInfo){
                    if(wt.IsWord()){
                        result.Append( wt.word + " ⇒ "  );
                    }
                }
                
                // 翻訳用前処理
                TranslationPreproc(wt);

                // wtから翻訳済みテキストを取り出す
                string translated =  wt.GetTranslatedText();
                // 翻訳できなかったら元文字をそのまま返す 
                if(translated == "") {  translated = wt.word ; }

                result.Append( translated  );
                
                // debuginfo is True
                if(this.debugInfo){
                    result.Append( "\n" );
                    MakeDebugInfo(result, wt);
                }
                
                // 次のWordTableへ
                wt = wt.next ;  
            }
            
            return result.ToString(); 
        }        

        //-------------------------------------------------------------------
        private void MakeDebugInfo(StringBuilder result, WordTable wt)
        {
            string indent = "    ";
            
            result.Append(indent);
            result.Append( " (" + wt.charCategory  + ") ");
            result.Append( " (" + wt.posCategory   + ") ");
            result.Append( " (" + wt.GetWordCost() + ") ");
            result.Append( " (" + wt.divided       + ") ");

            if(wt.IsSentenseHead){
                result.Append( " (sentense head)" );
            }
            if(wt.IsSentenseTail){
                result.Append( " (sentense tail)" );
            }
            result.Append("\n");

            SearchResult sResult = wt.sResult ;
            if(sResult != null && sResult.documents != null){
                foreach ( DocumentData ddata in sResult.documents  ){
                    result.Append(indent);
                    result.Append(ddata.GetData("key2"));
                    result.Append(" " + ddata.GetData("cost2"));
                    result.Append(" " + ddata.GetData("src"));
                    result.Append("\n");
                }
            }else{
                result.Append( "\n"  );
            }
            
            
            
        }
        
        //-------------------------------------------------------------------
        // 翻訳用前処理
        //   ・動詞／形容動詞区別
        private void TranslationPreproc(WordTable wt)
        {
            // 形容動詞解析
            string nounJpWord = "";
            nounJpWord = IsNounWithAdverb(wt) ;

            if(nounJpWord != ""){
                if(wt.next != null){
                    // 次の語は形容動詞の活用形？
                    string adverbJpWord = "";
                    adverbJpWord = IsAdverbTail(wt.next) ;
                    if(adverbJpWord != ""){
                        if(wt.transWord == "" &&
                           wt.next.transWord == ""){
                            wt.transWord      = nounJpWord;
                            wt.next.transWord = adverbJpWord;
                        }
                    }
                }
            }
        }        
        //-------------------------------------------------------------------
        // 形容動詞の語幹となる名詞なら，その日本語訳を返す
        private  string IsNounWithAdverb( WordTable wTable ){
            if(wTable.sResult == null){
                return "";
            }
            if(wTable.sResult.documents == null){
                return "";
            }
            // wTableはハダ動詞(動詞・形容動詞)になる名詞？
            foreach ( DocumentData sdata in wTable.sResult.documents  ){
                string src = sdata.GetSrc();
                if(src == "hyam(noun.adverb)"){
                    return sdata.GetData("key2") ;
                }
            }
            return "";
        }
        //-------------------------------------------------------------------
        // 形容動詞の語尾か？
        private  string IsAdverbTail( WordTable wTable ){
            if(wTable.sResult == null){
                return "";
            }
            if(wTable.sResult.documents == null){
                return "";
            }

            // wTableはハダ形容動詞の語尾？
            foreach ( DocumentData sdata in wTable.sResult.documents  ){
                string src = sdata.GetSrc();
                if(src == "hyam(adverb.hada.parts)"){
                    return sdata.GetData("key2") ;
                }
            }
            
            return "";
        }

        //-------------------------------------------------------------------
        // WordChainの各WordTableから翻訳結果を取り出し翻訳後文字列を返す
        //   (ブラウザ用)
        public String Chain2String_for_Web(WordChain wc)
        {
            StringBuilder     result;       // 出力文字列
            result = new StringBuilder();
            
            
            string translatedText = "";
            string originalText   = ""; // 先頭にマーカ


            // 語のchainを先頭から舐め、翻訳結果をresultに順次追加。
            WordTable wt = wc.Head;
            
            while ( wt!=null )
            {
                
                // タグの内容のテーブル
                if( wt.charCategory == CharCategory.HTMLtag ){
                    
                    if(translatedText.Trim() != ""){
                         
                        if(String.Compare(originalText , translatedText) != 0 ) {
                            // 翻訳データを埋める
                            originalText   += ( "<font color=blue size=-1>" +
                                                 translatedText + "</font>"   ) ;
                        }else{
                            // NOP
                            //  英文など無変換のものは併記しない
                        }
                        
                        result.Append( originalText );

                        // バッファクリア
                        originalText   = "";
                        translatedText = "";
                    }
                    
                    // htmlタグのwordをそのまま追加
                    result.Append( wt.word );

                  // この書き換えは効かない... [2009/09/26 09:56:07]
             //       string noblank = wt.word.Replace("target=\"_blank\"", "");
             //       result.Append( noblank );
                    
                    wt = wt.next ;  
                    continue;
                }
                
                
                // htmlタグの外 (訳すべきテキスト)
                
                // 原文の退避
                originalText   += wt.word;

                
                // 翻訳用前処理
                TranslationPreproc(wt);

                // wtから翻訳済みテキストを取り出す
                string translated =  wt.GetTranslatedText();
                // 翻訳できなかったら元文字をそのまま返す 
                if(translated == "") {  translated = wt.word ; }
                
                translatedText += translated ;
                
                
                wt = wt.next ;  
            }

            
            return result.ToString(); 
        }        

        
    } // End class StringTrans
    

} // End namespace KJ_dict

