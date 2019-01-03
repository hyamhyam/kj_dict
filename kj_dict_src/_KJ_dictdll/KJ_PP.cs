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
using System.Diagnostics ;
using HYAM.Lingua;

namespace HYAM.KJ_dict
{
    

    //-------------------------------------------------------------------------
    // 助詞関係の解析 
    //
    //  (1)助詞を翻訳・検知できなければ入力をそのまま返す。
    //  (2)助詞を翻訳したら翻訳語を入れた助詞のテーブルを返す。
    //  (3)助詞を分離した場合はその前の語を返す。
    //       会社員が  ---> 会社員 + が
    //                     「会社員」を返す。
    //                              「が」は品詞情報の設定まで。
    //                                前の語が確定していないと助詞も確定しないため。
    //       회사원이   ---> 회사원  + 이
    //                     「회사원」を返す。
    //                              「이」は翻訳情報の設定を行う。
    //
    class KJ_pp {
        
        static public WordTable Scan(WordChain wChain , WordTable wTable){

            if(wTable.IsTranslated()){
                return wTable;  // 変換済みなら何もしない
            }

            if(KJ_dict.inputIsHangul){
                wTable  = ScanPPKr(wChain, wTable);
            }else{
                wTable  = ScanPPJp(wChain, wTable);
            }
            
            return wTable;
        }

        //-------------------------------------------------------
        // 語のchainを先頭から舐め、助詞(Postpositional Particle)を分解する。
        static private WordTable ScanPPKr( WordChain wChain, WordTable wTable ){
            
            bool inputIsHangul = KJ_dict.inputIsHangul;
            
            // 語を舐め、助詞を分解する。

            if(wTable.charCategory!=CharCategory.Hangul &&
               wTable.charCategory!=CharCategory.LetterMix ){
                // ハングルでないなら、または英字＋ハングルでないなら何もしない。
                return wTable;
            }

            
            // 完全一致の助詞なら情報をセットし、終了
            string pp_trans = CheckHangulPP_Full(wTable) ;
            if(pp_trans != ""){
                wTable.transWord = pp_trans ;
                wTable.posCategory = PosCategory.PP;
                if(wTable.word.Length > 2){
                    wTable.Cost = 0;
                }else if(wTable.word.Length == 2){
                    wTable.Cost = 3;
                }else{
                    wTable.Cost = 5;
                }
                
                return wTable;
            }

            // wordTableが辞書に完全一致で存在するか調べる。
            bool isExist = KJ_dict.CheckFull( wTable.word );
            if( isExist ){
                // 完全一致した語は判定せず、なにもしない。
                return wTable;
            }

            
            // 助詞の分解を試みる
            WordChain resultChain = DivideHangulPP( wTable );
            if(resultChain==null){
                // 助詞はついてなかった。なにもしない。
                return wTable;
            }
            
            // resultChainは 語＋助詞のチェーン。WordTと入れ替える
            wChain.Swap(wTable, resultChain);
            
            // 助詞で始まる熟語でないかチェック
            WordTable idiomwt = Idiom.Search( wChain, resultChain.Tail );
            
            // resultChainの先頭は未処理。
            // いったんcurrentをresultChainの先頭に戻す
            //     위원회를 --->    위원회          +   를
            //      wTable        resultChain.Head     [を]:翻訳済み
            wTable = resultChain.Head; 

            
            return (wTable);
        }


        //-------------------------------------------------------
        // wordがハングルの助詞と完全一致なら、訳語をreturn で返す。
        static private String  CheckHangulPP_Full(WordTable wordT) {
            string word = wordT.word;
            string prevword = "";

            if(wordT.prev == null){
                return "";
            }
            
            // 前の語が空白なら助詞とは判断しない。
            //     초파리에서 이 유전자를 の「이」は「が」ではない
            if( !wordT.prev.IsWord() ){
                return "";  // 何もせず抜ける
            }
            
            prevword = wordT.prev.word ;  // 前の語
            
            // 前の語がハングルならつながっているから
            // このタイプの判断が必要なのは前が英数字
            //   ex.    MP3의 
            return KJ_Filter.SearchPPall(word);
            
        }
        

        //-------------------------------------------------------
        // wordが助詞交じりの場合、助詞を分離する。
        //   본부는  ---> 본부  +  는      
        //   word         head     tail    
        //               
        //     ただし headは辞書になければいけない（未知語不可）
        //     もしくはheadは英字のみ。( KOTRA는 等)          
        //               
        //
        static private WordChain  DivideHangulPP( WordTable wTable ) {
            
            StringDivider sd = new  StringDivider(wTable.word);
             
            string trans;

            // 前から1文字ずつ落としつつ，検索。    
            while( sd.eof() == false ) {
                // 文字列を分割する
                HYAM.KJ_dict.Pair pair = sd.DivideForward() ;
                
                
                if(Hangul.withPachim(pair.head)){
                    trans = KJ_Filter.SearchPP_Pachim(pair.tail) ;
                }else{
                    trans = KJ_Filter.SearchPP_NoPachim(pair.tail);
                }
                
                if(trans == ""){
                    continue;
                }

                SearchResult result=null;
                
                result = KJ_dict.SearchFull( pair.head );
                
                
                if(result != null ){
                    // 助詞確定
                    
                    // 助詞に先行しない語は除外
                    //   산은  （은が助詞なら、산は動詞[連体詞]ではない）
                    //   (result作り直し)
                    //  result = SearchResult.CheckPPConnectable(result);
                    
                    // もし語が全部落ちたならば何もしない
                    if(result.documents.Count == 0){
                        // nop
                    }else{
                    
                        // 先行語＋助詞のチェーンを作る
                        WordTable wt1 = new WordTable(pair.head);
                        wt1.SetResult(result);
                        wt1.divided = Divided.Lead; 
                                                    
                        WordTable wt2   = new WordTable(pair.tail, trans);
                        wt2.posCategory = PosCategory.PP;
                        wt2.divided     = Divided.Trail; 
                        
                        // 長い助詞ほどコストを低くする
                        // wt2.Cost      = 0;
                        if(wt2.word.Length > 2){
                            wt2.Cost = 0;
                        }else if(wt2.word.Length == 2){
                            wt2.Cost = 3;
                        }else{
                            wt2.Cost = 5;
                        }
                        
                        WordChain rtnChain = new WordChain(wt1, wt2);
                        
                        return rtnChain;
                    }
                }
                
            }  // end of while          


            return null;  // 分離できなかったらnullを返す。
        }                
        
        //-------------------------------------------------------
        // 辞書チェックなしのハングル助詞チェック
        // 
        //    末尾が助詞と一致するなら分離する。
        // 
        static  public  WordChain CheckPPwithoutDictKr(WordTable wt) {
            
            if(wt.IsTranslated() ){
                // 翻訳済みなら何もしない。
                return null; 
            }
            if(wt.charCategory!=CharCategory.Hangul &&
               wt.charCategory!=CharCategory.LetterMix ){
                // ハングルでないなら、または英字＋ハングルでないなら何もしない。
                return null; 
            }

            WordTable rtnWt = wt ;  // 戻りのdefault

            StringDivider sd = new  StringDivider(wt.word);

            string trans = "";

            // 前から1文字ずつ落としつつ，検索。    
            while( sd.eof() == false ) {
                // 文字列を分割する
                HYAM.KJ_dict.Pair pair = sd.DivideForward() ;

                if(Hangul.withPachim(pair.head)){
                    trans = KJ_Filter.SearchPP_Pachim(pair.tail) ;
                }else{
                    trans = KJ_Filter.SearchPP_NoPachim(pair.tail);
                }

                if(trans == ""){
                    continue;
                }

                // wtをwt1とwt2に分割
                WordTable wt1 = new WordTable(pair.head);
                wt1.divided = Divided.Lead ; 

                //  分離できた。 wT2は助詞と仮定。訳語も入れておく
                WordTable wt2 = new WordTable(pair.tail, trans);  
                wt2.posCategory  = PosCategory.PP;
                wt2.Cost = 2;
                wt2.divided = Divided.Trail ; 

                
                WordChain rtnChain; // 返却チェーン
                // wt1を調査
                //  (未知語なので分割してみる)
                WordChain wc1 = KJ_Analyzer.WordPartProc(wt1);
                
                if(wc1==null){
                    // 分割できなかった
                    rtnChain = new WordChain(wt1, wt2);
                }else{
                    wc1.Add(wt2);
                    rtnChain = wc1;
                }
                
                return rtnChain;
            }

            return null;  // 分離できなかったらnullを返す
        }                

        
        //-------------------------------------------------------
        // 語のchainを先頭から舐め、助詞(Postpositional Particle)を分解する。
        static private WordTable ScanPPJp(WordChain wChain, WordTable wTable ){
            
            // 語を舐め、助詞を分解する。
            
            if(wTable.IsTranslated() || wTable.IsDigit() ){
                // 翻訳済み または 数字ならなにもしない。
                return wTable;  
            }

            // J-->K方向の場合、ハングルは
            // 前の語に依存するので訳語(ハングル)の設定は別途。
            // 助詞である事の確定だけ試みる
            
            // 分離されたひらがなの助詞判定だけ
            PosCategory pp = CheckPos_Hira(wTable.word) ;
            if(PosCategory.PP == pp ){
                wTable.posCategory=pp;
            }

            // 漢字＋ひらがなの形での助詞分離
            if(wTable.charCategory == CharCategory.KanHira) {
                WordChain resultChain = DivideJapanesePP(wTable);
                if(resultChain!=null){
                    wChain.Swap(wTable, resultChain);
                    // 前の語は未翻訳なのでそれを返す
                    wTable = resultChain.Head;
                }
            }
                
            
            return (wTable);
        }


        //-------------------------------------------------------
        static private WordChain  DivideJapanesePP( WordTable wTable ) {
            StringDivider sd = new  StringDivider(wTable.word);
            
            // 前から1文字ずつ落としつつ，検索。
            string trans = "";
            while( sd.eof() == false ) {
                // 文字列を分割する
                HYAM.KJ_dict.Pair pair = sd.DivideForward() ;

                if(Hangul.withPachim(pair.head)){
                    trans = KJ_Filter.SearchPP_Pachim(pair.tail) ;
                }else{
                    trans = KJ_Filter.SearchPP_NoPachim(pair.tail);
                }
                
                if(trans == ""){
                    continue;
                }
                
                SearchResult result = KJ_dict.SearchFull( pair.head );
                
                if(result != null ){
                    // 助詞確定
                    
                    // 助詞に先行しない語は除外
                    //   산은  （은が助詞なら、산は動詞[連体詞]ではない）
                 //   result = SearchResult.CheckPPConnectable(result);
                    
                    // もし語が全部落ちたならば何もしない
                    if(result.documents.Count == 0){
                        // nop
                    }else{
                    
                        // 先行語＋助詞のチェーンを作る
                        WordTable wt1 = new WordTable(pair.head);
                        wt1.SetResult(result);
                        wt1.divided = Divided.Lead; 
                        
                        // wt2.transWord はまだ設定しない。
                        //   前の訳語のパッチムに影響されるため。                            
                        WordTable wt2   = new WordTable(pair.tail);
                        wt2.posCategory = PosCategory.PP;
                        wt2.divided     = Divided.Trail; 
                        
                        // 長い助詞ほどコストを低くする
                        // wt2.Cost      = 0;
                        if(wt2.word.Length > 2){
                            wt2.Cost = 0;
                        }else if(wt2.word.Length == 2){
                            wt2.Cost = 3;
                        }else{
                            wt2.Cost = 5;
                        }
                        
                        WordChain rtnChain = new WordChain(wt1, wt2);
                        
                        return rtnChain;
                    }
                }
                
            }  // end of while          


            return null;  // 分離できなかったらnullを返す。
        }                
        



        //----------------------------------------------------------------
        // J-->K方向の助詞翻訳
        static public string  TransJapanesePP(WordTable wt){
            
            // ここにくるという事は posCategory == PosCategory.PPは確定している。
            
            // 1つ前の語に対応する韓国語を取り出す（パッチム判定のため）
            string prevtrans="";
            if(wt.prev.transWord != ""){
                prevtrans = wt.prev.transWord;
            }else{
                prevtrans = wt.prev.GetTranslatedText();
            }
            
            // J-->K方向の助詞変換
            // こちらは前の語を翻訳しないと、変換すべき語を判断できない。
            if(Hangul.withPachim(prevtrans)){
                return KJ_Filter.SearchPP_Pachim(wt.word);
            }else{
                return KJ_Filter.SearchPP_NoPachim(wt.word);
            }
            
        }

        //-------------------------------------------------------
        // textが日本語の助詞ならば、Category.PPを返す。
        static public PosCategory  CheckPos_Hira(string input ){
            
            if(KJ_Filter.SearchPPall(input) != ""){
                return(PosCategory.PP);
            }else{
                return PosCategory.Null ;
            }
            
        }
        
        
        
    } // end of class 

} // End namespace KJ_dict
