// Copyright 2004, 2010 hyam <hyamhyam@gmail.com>
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
using System.Diagnostics ;
using HYAM.Lingua;

namespace HYAM.KJ_dict
{
    

    //-------------------------------------------------------------------------
    // adhocな形態素解析+翻訳 
    public class KJ_Analyzer {
        
        StringTrans   sTrans;

        #if DEBUG_LOG
        // debug logファイル用
        static private FileStream   log_fs=null ;
        static private StreamWriter log_w=null;
        #endif

        //-------------------------------------------------------
        // constructor
        public KJ_Analyzer (StringTrans stringTrans) {
             this.sTrans = stringTrans;

        }
        //-------------------------------------------------------
#if DEBUG_LOG
        static public void StartDebugLog () {
            // logのOpen
            if(log_fs==null){
                log_fs  = new FileStream("Log_Analyzer.txt", 
                                         FileMode.Create, 
                                         FileAccess.Write);
                log_w   = new StreamWriter(log_fs, Encoding.UTF8);
            }
        }
        //-------------------------------------------------------
        static public void WriteDebugLog (string logText) {
            if(log_w!=null){
                log_w.Write ("## " + logText + "\n" );
                log_w.Flush();
            }
        }
        //-------------------------------------------------------
        static public void EndDebugLog () {
            if(log_w!=null){
                log_w.Close();
                log_fs=null;
            }
        }
#endif            
        //-------------------------------------------------------
        // 翻訳のメイン
        //     KJ_browserの中にも同じものをコピーで持っている点注意
        //
        public WordChain MorphologicScan( WordChain wChain ){
                
            #if DEBUG_LOG
            KJ_Analyzer.StartDebugLog();
            KJ_Analyzer.WriteDebugLog("MorphologicScan");
            #endif            

            if(!wChain.Exist()){
                return(wChain); 
            }
            
            WordTable wTable = wChain.Head;

            // 解析ループ  1回目
            // 翻訳前処理（助詞分離など先に  熟語の精度を上げるため）
            //  고정 관념을 ---> 고정 관념 을に分離しておくため(熟語判定用)
            while ( wTable != null )
            {
                this.sTrans.pBarStep(); // プログレスバーを進める
                
                
                // 1回目処理
                wTable = firstScan(wChain, wTable);
                
                wTable = wTable.next ;  // 次の語へ

            } // 翻訳前処理ここまで
            

            // 解析ループ  2回目  (翻訳のメインループ)
            wTable = wChain.Head;  //   再度先頭から
            while ( wTable != null )
            {
                this.sTrans.pBarStep(); // プログレスバーを進める

        
                // 2回目処理
                wTable = secondScan(wChain, wTable);
        
                wTable = wTable.next ;  // 次の語へ
                
            } // 翻訳のメインループ ここまで
            
            #if DEBUG_LOG
            KJ_Analyzer.EndDebugLog();
            #endif            

            return (wChain);
        }
        
        //--------------------------------------------------------------
        public WordTable firstScan(WordChain wChain, WordTable wTable){
                // 速度向上のため、skipさせる
                if( wTable.IsTransSkipWord() ) 
                {
                    return wTable;   // 何もせずreturn
                }
                
                // 前処理
                wTable = ChainPreProc(wChain, wTable);    
        
                // 数助詞，冠数詞をチェック
                wTable = Numerative.Scan(wChain, wTable);    

                // 助詞の解析を試す (前の語の辞書引きあり)
                wTable = KJ_pp.Scan(wChain, wTable);    //2005.08.26 数詞の前に

                return wTable;
        }
        //--------------------------------------------------------------
        public WordTable secondScan(WordChain wChain, WordTable wTable){
                // 速度向上のため、skipさせる
                if( wTable.IsTransSkipWord2() ) // 少しゆるいskip
                {
                    return wTable; // 何もせずreturn
                }
                
                // 熟語をチェック
                wTable = Idiom.Scan(wChain, wTable);    
                
                // 分割なし完全一致の語
                wTable = DelimitedWord.Scan(wChain, wTable);    
                
                // 分割も考慮し詳しい(?)解析
                //    (辞書引きなし助詞解析あり)
                //     分解した最後のwTableを返す
                wTable = ScanDetail(wChain, wTable);  

                return wTable;
        }
        //--------------------------------------------------------------
            
        
        //------------------------------------------------------------------
        // 分割も考慮し詳しい(?)解析
        //    分解後チェーンの最後のwTableを返す
        //
        //             
        //    PalDalHan   WordTable   ←input
        //      ↓
        //    PalDal      WordTable
        //    Han         WordTable   ←return
        //
        private  WordTable ScanDetail( WordChain wChain, WordTable wTable ){

            if(wTable.IsTranslated()){
                return wTable;  // 変換済みなら何もしない
            }

            #if DEBUG_LOG
            KJ_Analyzer.WriteDebugLog("ScanDetail");
            #endif            

            // 各国固有部
            if(KJ_dict.inputIsHangul){
                WordTable rtnWt = ScanDetailKr(wChain,  wTable);
                if(rtnWt!=null){
                    return rtnWt ;
                }
            }else{
                WordTable rtnWt = ScanDetailJp(wChain,  wTable);
                if(rtnWt!=null){
                    return rtnWt ;
                }
            }


            // 語分割ありで、熟語をチェック
            WordTable idiomwt = Idiom.Search2( wChain, wTable);  
            if(idiomwt != null){
                // 熟語が解決できた
                return idiomwt ;
            }
            
            // 語の分解    
            WordChain devChain = WordDivider(wTable);
            if(devChain!=null){ // 分解できた？
                if(AcceptableTransWord(wTable, devChain) ){  // 採用できるか？

                    // 採用できるならwTableを分解したチェーンdevChainで入れ替え
                    wChain.Swap(wTable, devChain);

                    return devChain.Tail ;       // 分解した最後の位置へ
                }
            }    

            // それでも変換なしならカタカナまたはハングルに自動変換
            wTable = ConvAutoConv(wTable);

            return wTable ;
        }
        

        
        
        //--------------------------------------------------------------------
        // wTableを翻訳した結果のチェーンが使えるかどうか？
        private  bool AcceptableTransWord( WordTable wTable, WordChain wChain ){
            
            // 3文字以上で、翻訳語の同じチェーンなら不採用
            if(wTable.word.Length > 2 &&
               wTable.word.Length == wChain.Length){
                return false;
            }
        
            // 翻訳済み長が全体の3分の1以下なら不採用
            if(wChain.TranslatedCharCount() <= wTable.word.Length/3 ) {
                return false;
            }
            
            return true;  // 採用可
        }
        //-------------------------------------------------------------------
        // 
        private  WordTable ScanDetailKr( WordChain wChain, WordTable wTable ){
            
            if(wTable.posCategory == PosCategory.PP ){
                //  PosCategory.PPが設定されているK->J方向なら助詞は翻訳済み。
                return wTable ;
            }
            
            return null;
        }
                    
        //-------------------------------------------------------------------
        // 
        private  WordTable ScanDetailJp( WordChain wChain, WordTable wTable ){
            
            //  助詞を処理  
            //   (助詞で始まる熟語があるので熟語の後）
            if(wTable.posCategory == PosCategory.PP ){
                // 改めて訳語を設定
                string pp = KJ_pp.TransJapanesePP(wTable);
                if(pp != ""){
                    // 訳語設定
                    wTable.transWord = pp; 
                    wTable.Cost = 0;
                    
                    // ハングルの助詞なら、分かち書きのため空白を置く。
                    wTable.transWord = wTable.transWord + " ";             
                    return wTable;
                }
            }
            
            return null;
        }
                    
        
        //------------------------------------------------------------------
        private  WordChain WordDivider( WordTable wTable ){
            
            WordChain wc1 = null;
            
            #if DEBUG_LOG
            KJ_Analyzer.WriteDebugLog("WordDivider");
            #endif            
            
            // 辞書検索なしの助詞チェック
            if(KJ_dict.inputIsHangul){
                wc1 = KJ_pp.CheckPPwithoutDictKr(wTable) ;
            }

            // 3文字の「ハングルまたは漢字」なら特別チェック
            if(wTable.word.Length==3 && 
                 ( CodeCheck.IsHangul(wTable.word) ||
                   CodeCheck.IsKanji(wTable.word) )
              ) 
            {
                
                WordChain wcThree = Check3Chars(wTable);
                if(wcThree != null){
                    return wcThree;
                }
            }
            

            // 全体でmatchしないなら部分文字列で検索（再起 語分解あり）
            WordChain wc2 = null;
            wc2 = WordPartProc(wTable) ;
            
            // wtは最後の語テーブル。未変換なら再調査
    //        WordChain wc3 = CheckLastWord(wChain, wTable);
            

            // costが小さい方を採用
            WordChain rtnWc = WordChain.GetMinimunCostChain(wc1, wc2);
            
            return rtnWc;
        }
        
        //--------------------------------------------------------------------
        // 語の変換   （語の分解あり)
        //   語分解で翻訳済み末尾がずれることがあるのでWordTableを返す
        //           ABCDEF という語に対し、
        //              ABCDE + F
        //              ABCD  + EF
        //              ABC   + DEF
        //               :       :   
        //   と調べていき、最小コストの訳語がとれる分割を採用する
        //
        static public WordChain  WordPartProc(WordTable wt){
            SearchResult sResult;
            String str = wt.word;
            

            if(wt == null){
                return null;
            }
            if(wt.word!=null  &&  wt.word.Length==1){   // 1文字検索はしない
                return null;
            }

            if(wt.IsTranslated() || wt.IsDigit()  ){
                return null; // 翻訳済みまたは 数字なら 何もしない
            }
            
            #if DEBUG_LOG
            KJ_Analyzer.WriteDebugLog("WordPartProc:" + wt.word);
            #endif            
            
            
            // 再起に備え，まず完全一致で検索 
            sResult = KJ_dict.SearchFull(str);
            if(sResult != null){
                wt.SetResult(sResult); // 全体でmatch
                WordChain wc = new WordChain(wt);
                return wc;    
            }
            
            // 部分語の2文字は分割しない
            if(wt.word!=null     &&  
               wt.word.Length==2 && 
               wt.divided != Divided.Non ){   
                return null;
            }

            // 3文字の「ハングルまたは漢字」なら特別チェック
            if(wt.word!=null && wt.word.Length==3 && 
               wt.divided != Divided.Non  &&  // 部分語の3文字語は除外
                 ( CodeCheck.IsHangul(wt.word) || 
                   CodeCheck.IsKanji(wt.word)     ) 
              )
            {
                
                WordChain wc3 = Check3Chars(wt);
                if(wc3!=null){
                    return wc3;
                }
            }
            
//            int target_len=2;   // 2005.09.03 やっぱり1文字まで切らないとだめだ
            int target_len=1;  
            int str_len = str.Length  ;

            int minimumCost = 9999;
            WordChain minimumCostChain = null;
            
            // 前から1文字ずつ落としつつ，検索。
            
            while(true){
                int start = str_len - target_len ;
//                if( start <= 1 ){  // 2005.09.03
                if( start <= 0 ){
                    break;
                }
                // 文字列を分割する
                // str --> str2 + str3
         //       String str2 = str.Remove   (start,  target_len );
         //       String str3 = str.Substring(start,  target_len );
                // 前から分割に変えた(2005.08)
                String str2 = str.Remove   ( target_len, start);
                String str3 = str.Substring( target_len, start); //あとできれいに
                
                // 前と後ろを、それぞれ検索        
                WordChain wc2  = DividedWordSearch(str2);    
                WordChain wc3  = DividedWordSearch(str3);    
                
                #if DEBUG_LOG
                KJ_Analyzer.WriteDebugLog("str2/str3:" + str2 + "/" + str3);
                #endif            

                WordTable wt2, wt3;
                if(wc2==null){
                    wt2 = new WordTable(str2);
                    wc2 = new WordChain(wt2);
                }
                if(wc3==null){
                    wt3 = new WordTable(str3);
                    wc3 = new WordChain(wt3);
                }
               
                // 分割情報設定
                if(wt.divided == Divided.Non){
                    wc2.Head.divided = Divided.Lead ;
                    wc3.Head.divided = Divided.Trail ;
                }
                if(wt.divided == Divided.Lead){
                    wc2.Head.divided = Divided.Lead ;
                    wc3.Head.divided = Divided.Middle ;
                }
                if(wt.divided == Divided.Middle){
                    wc2.Head.divided = Divided.Middle ;
                    wc3.Head.divided = Divided.Middle ;
                }
                if(wt.divided == Divided.Trail){
                    wc2.Head.divided = Divided.Middle ;
                    wc3.Head.divided = Divided.Trail ;
                }
                
                // wc2とwc3をつなぐ
                wc2.Add(wc3);

                //  wc2---wc3 のコストを計算
                int divChainCost = wc2.GetChainCost();
                if(minimumCost >=  divChainCost){
                    minimumCostChain =  wc2 ;
                    minimumCost      =  divChainCost ;  //最小コストの更新
                }

                #if DEBUG_LOG
                KJ_Analyzer.WriteDebugLog("wc2:" + wc2.Head.word + "," + wc2.Head.Cost);
                KJ_Analyzer.WriteDebugLog("wc3:" + wc3.Head.word + "," + wc3.Head.Cost);
                KJ_Analyzer.WriteDebugLog("divChainCost:" + divChainCost);
                #endif            
                
                
                target_len++;
                
            } // end of while
            
            
            // Chain中のwordが全て翻訳できていない
            if(minimumCostChain==null  ||
               ( minimumCostChain!=null && !minimumCostChain.IsTranslated() ) ){
                return null; 
            }
            
            // 翻訳できていない部分chainを再起実行
            WordTable subT=minimumCostChain.Head;
            while(subT!=null){
                if(!subT.IsTranslated()){
                    WordChain subWc = WordPartProc(subT);
                    if(subWc!=null){
                        WordTable wNext = subT.next;
                        minimumCostChain.Swap(subT, subWc);
                        subT = wNext;
                        continue;
                    }
                }
                subT=subT.next;
            }
            
            return minimumCostChain;
        }

        //--------------------------------------------------------------------
        // 完全一致の辞書検索 (再起にすると時間がかかりすぎるので再起なし)
        //
        static public WordChain  DividedWordSearch(string str){
            WordTable wt;
            WordChain wc;
            
            #if DEBUG_LOG
            KJ_Analyzer.WriteDebugLog("DividedWordSearch:" + str);
            #endif            
            
            SearchResult sResult = KJ_dict.SearchFull(str);  
            
            if(sResult==null){
                // strが辞書にない
                // wc = WordPartProc(wt);
                return null;  
            }
            
            wt = new WordTable(str);
            wc = new WordChain();

            // 検索できた
            wt.SetResult(sResult); // wt3に検索結果を入れる
            //wt.IsDevided=true;  // 分割語であるマーク
                
            wc.Add(wt);
            
            return wc ;
        }
        


        
        //------------------------------------------------------------------------
        // 辞書に見つからなかった語を自動変換する
        //   ハングル <---> カタカナ
        private WordTable ConvAutoConv(WordTable wt)
        {
            if(wt.IsTranslated()){
                return wt;  // 変換済みなら何もしない
            }

            if(!this.sTrans.AutoConv){
                return wt;  // AutoConvをoffにしている場合は何もせず抜ける。
            }
            
            if(KJ_dict.inputIsHangul){
                // K-->J方向
                if(CodeCheck.IsHangul(wt.word)){
                    wt.transWord   = Hangul.Hangul2Kana(wt.word);
                }
            }else{
                // J-->方向
                if(wt.charCategory == CharCategory.Katakana){
                    // カタカナの場合
                    wt.transWord   = Kana.Kana2Hangul(wt.word);
                }else{
                    if(wt.charCategory == CharCategory.Hiragana){
                        // ひらがなの場合
                        wt.transWord   = Kana.Hira2Hangul(wt.word);
                    }
                }
            }
            
            return wt;
        }
        
        //-------------------------------------------------------------------
        // チェーンの前処理。 
        //     句読点の判定、置換など
        //   
        private WordTable ChainPreProc(WordChain wChain, WordTable wTable)
        {
            if(wTable.IsTranslated()){
                return wTable;  // 変換済みなら何もしない
            }
            
            if(wTable.charCategory==CharCategory.Letter){
                // 一種の正規化  
                if(wTable.word=="ㆍ"){      // 0x318D  韓国で使われる中点
                    wTable.word = "・";      // 0x30FB  日本語の全角中点
                }
            }
            
            if(wTable.charCategory==CharCategory.Punctuation){
                if(KJ_dict.inputIsHangul){
                    if(wTable.word=="."){
                        if(wTable.next==null || !wTable.next.IsWord() ){
                            wTable.transWord = "。";
                        }
                    }
                }else{
                    if(wTable.word=="、"){
                        wTable.transWord = ", ";
                    }
                    if(wTable.word=="。"){
                        wTable.transWord = ". ";
                    }
                }
            }

            return (wTable);
        }
        

        //------------------------------------------------------------------
        // 3文字語の調査 （部分分割後も使う）
        static private  WordChain Check3Chars( WordTable wTable ){
            SearchResult sResult;
            
            // ABC --->  AB + C
            String head    = wTable.word.Remove   (2, 1);
            String suffix  = wTable.word.Substring(2, 1);
            sResult = KJ_dict.SearchFull(head);
            if(sResult != null){
                // lastWordListにあるか調べる
                string translatedSuffix = KJ_Filter.SearchSuffix(suffix);
                if(translatedSuffix != ""){
                    WordTable headWt = new WordTable(head);
                    headWt.SetResult(sResult); 
                    WordChain wc = new WordChain(headWt);

                    WordTable wt = new WordTable(suffix, translatedSuffix);
                    wt.Cost = 5;
                    wc.Add(wt);
                    return wc;    
                }
            }

            // ABC --->  A + BC
            String prefix  = wTable.word.Remove   (1, 2);
            String tail    = wTable.word.Substring(1, 2);
            sResult = KJ_dict.SearchFull(tail);
            if(sResult != null){
                // lastWordListにあるか調べる
                string translatedPrefix = KJ_Filter.SearchPrefix(prefix);
                if(translatedPrefix != ""){
                    WordTable wt = new WordTable(prefix, translatedPrefix);
                    wt.Cost = 5;
                    WordChain wc = new WordChain(wt);

                    WordTable tailWt = new WordTable(tail);
                    tailWt.SetResult(sResult); // 全体でmatch
                    
                    wc.Add(tailWt);
                    return wc;    
                }
            }
            
            return null ;
        }
        
    } // end of class KJ_Analyzer

} // End namespace KJ_dict
