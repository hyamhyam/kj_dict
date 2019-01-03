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
    

    //--------------------------------------------------------------------
    class Idiom {
         
        //-------------------------------------------------------
        // 語のchainを先頭から舐め、熟語を探す。
        static public WordTable Scan( WordChain wChain, WordTable wTable ){
            
          // 熟語は優先のため変換済みチェックなし  
            //if(wTable.IsTranslated()){
            //    return wTable;  // 変換済みなら何もしない
            //}
            
             // wtで始まる熟語のチェック
             WordTable idiomwt = Idiom.Search( wChain, wTable);
             
             if(idiomwt != null){
                 // 熟語が解決できた
                 return idiomwt;
             }
            
            return wTable;
        }
        
            
        
        //--------------------------------------------------------------
        // startwtで始まる熟語を最長一致でチェックする
        //
        // (before)
        //  -----wt1-----wt2-----wt3-----wt4-----wt5-----wt6-----
        //               ↑              ↑
        //              startwt         matched
        //      
        // (after)
        //  -----wt1-----idiomwt-----wt5-----wt6-----
        //
        //  チェーン中の熟語を成すテーブルは1つにまとめる。
        //    （正確には熟語のテーブルを挿入し、個々の語のテーブルは削除）
        //
        static public WordTable  Search ( WordChain wc, WordTable startwt){

         //熟語は語単位より優先
         //   if(startwt.IsTranslated()){
         //       return  null; // 翻訳済みの語は触らない
         //   }
            
            bool isExist; // 辞書中にチェック対象の語(熟語)が存在するか？
            
            WordTable idiomwt = null;  // 熟語テーブル
            SearchResult sResult;
            string word = startwt.word ; 
            
            // startwtが翻訳済みで助詞なら何もしない
            // 暫定的 2005.08.26
        //    if(startwt.IsTranslated() && startwt.pos == PosCategory.PP ){
        //        return(null);   // not found
        //  }
            
            // 前方一致で該当なしなら
            // この語で始まる熟語は辞書にない 
            isExist = KJ_dict.CheckForward( word );
            if(!isExist){
                return(null);   // not found
            }
            
            // 前方一致でmatchしたので先読みして熟語チェック
            String word2 = "" ;
            string next = "";
            WordTable nextTable = startwt.next;
            WordTable matched = startwt;
            
            // 次の語以降を足しながらチェック
            while(nextTable!=null) {
                if(nextTable == null || nextTable.word == ""){ 
                    break;  // 次の語なしなら抜ける
                }
                next = nextTable.word;   
                char ch =  next[0];
                if(Char.IsSeparator(ch)){
                    next = " ";
                }
                
                if(next == " "  ){  
                    word = word + next;
                    nextTable = nextTable.next; // 次の語へ
                    continue;    // 空白付加だけでは再検索しない
                }              
                   
                word2 = word + next;
                //次の語を足して前方一致チェック
                isExist = KJ_dict.CheckForward( word2 );
                if(!isExist){
                    // nextを足したら，一致しない。→ これ以上先を見る必要なし
                    
                    
                    break;
                }   
                
                // 一致した部分まででで完全一致検索
                sResult = KJ_dict.SearchFull(word2); 
                if(sResult != null ){
                   // string trimword = word2;
                   // trimword = trimword.Replace(" ", ""); // 空白削除
                    if(IsValidIdiom(sResult)) { // 長さ4以上でないと採用しない
                       // 熟語が見つかった                    
                       idiomwt = new WordTable(word2);
                       idiomwt.charCategory = startwt.charCategory; 
                                       // 先頭文字種で (混ざっている可能性あるが)
                       idiomwt.posCategory  = PosCategory.Idiom;
                       idiomwt.SetResult(sResult);
                       matched = nextTable;  // 完全一致した最後のテーブル
                    }
                }           
                     
                // さらに次の語へ
                word = word2;  // nextを足したものを元とする。
                nextTable = nextTable.next; // 次の語へ
                
            } // while loop
            
            
            if(startwt.word == word){
                // 熟語でなく単なる入力語の完全一致の時は返さない
                return(null);
            }   

            if(idiomwt != null) {
                // 熟語が見つかっている
                
                wc.Insert(matched, idiomwt); 
                
                // startwtからmatchedまでは消していい。
                WordTable  delwt = startwt;
                while(true){
                    wc.Delete(delwt);
                    if(delwt == matched){
                        break;
                    }
                    delwt = delwt.next;
                }
                
                // 熟語のテーブルを返す
                return(idiomwt);   
            }
            
            // 熟語なし
            return(null);
        }
        
        //----------------------------------------------------------------
        // 熟語として判断していいなら true
        static private bool IsValidIdiom(SearchResult sResult){
            bool rtn = false;
            foreach ( DocumentData sdata in sResult.documents  ){
                string original 
                    = sdata.GetKey(!KJ_dict.inputIsHangul);  // 原文
                string pos   = sdata.GetPos();
                if(original.Length <= 2 ){  //長さ2以下は無条件却下
                    continue;
                }
                if(original.Length <= 3 ){  //長さ3
                    if(pos.StartsWith("pn")){
                        // nop   長さ3以下の固有名詞は熟語としない
                        continue;
                    }
                }
                
                //長さ4以上なら採用
                rtn = true;
            }
            return rtn;
        }
        //----------------------------------------------------------------
        // 語分割ありの熟語チェック
        //     공부하지는 못할 것이라는==> 공부 + 하지는 못할 것이라는
        //     ~~~~~~~~~~                         ~~~~~~~~~~~~~~~~~~~~
        //      startwt                       ここのWordTableはdivided=Trail
        //
        static public WordTable  Search2(WordChain wc, WordTable startwt){
            String str = startwt.word;
            
            if(!startwt.IsWord()){
                return null;     // 空白などで始まる場合は、は処理しない
            }
            
            int target_len=1;  
            int str_len = str.Length  ;

            // 前から1文字ずつ落としつつ，熟語検索。
            while(true){
                if( target_len >= str_len ){
                    break;
                }

                // 文字列を分割する
                // str --> str2 + str3
                String str2 = str.Substring(0,  target_len );
                String str3 = str.Remove(0,  target_len );
                
                WordTable wt3 = new WordTable(str3);
                wt3.next = startwt.next;
                wt3.prev = startwt.prev;
                WordTable idiomTable = Idiom.Search(wc, wt3);
              
                if(idiomTable != null){
                    // 熟語確定
                    WordTable wt2 = new WordTable(str2);
                    wc.InsertBefore(idiomTable, wt2);
                    
                    WordChain wc2 = KJ_Analyzer.WordPartProc(wt2);
                    if(wc2!=null){
                        wc.Swap(wt2, wc2);
                    }
                    idiomTable.divided = Divided.Trail;
                    return idiomTable;
                }

                target_len++;
            }
            
            return null;
        }
        
        
    } // end of class KJ_Analyzer

} // End namespace KJ_dict
