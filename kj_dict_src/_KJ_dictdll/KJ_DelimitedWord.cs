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
    class DelimitedWord {
        
        
        //-------------------------------------------------------
        //  分割なし完全一致の語
        static public WordTable Scan(WordChain wChain , WordTable wTable ){
            
            if(wTable.IsTranslated()){
                return wTable;  // 変換済みなら何もしない
            }

             
            // 翻訳しなくていい文字は抜ける
            if(!wTable.IsWord() ||      // 空白や制御文字
               wTable.IsDigit() || 
               wTable.IsOther()    ){   // 記号
                return wTable;
            }
            
            
            // 助詞なら抜ける
            if(wTable.posCategory == PosCategory.PP ){
                return wTable;
            }

           
            // 独立した1文字の語は辞書より先に検索   (単独の이など)
            if(wTable.word.Length == 1){
                CheckOneWord(wTable);
                if(wTable.transWord != ""){
                    return wTable;    // match
                }
            }
           
           
            // 全体で完全一致で検索
            SearchResult sResult = KJ_dict.SearchFull(wTable.word);
            if(sResult != null){
                wTable.SetResult(sResult);
                return wTable;    // 完全一致で見つかった
            }
            

            // 4文字語の調査 （部分分割後は使わない）
            WordChain wc = Check4Chars(wTable);
            if(wc != null){
                wChain.Swap(wTable, wc);
                return wc.Tail;    
            }
            
            
            
            return wTable;
        }
        //------------------------------------------------------------------
        // 4文字語の調査 （部分分割後は使わない）
        static private  WordChain Check4Chars( WordTable wTable ){
            SearchResult sResult;
            
            if(KJ_dict.inputIsHangul){
                // 4文字以外は抜ける。
                if(wTable.word.Length != 4){
                    return null;
                }
                
                if(wTable.word.EndsWith("하게")){
                    string head = wTable.word.Remove(2, 2);  // 4文字なので固定
                    sResult = KJ_dict.SearchFull(head);
                    if(sResult != null){
                        WordTable tempwt = new WordTable(head);
                        tempwt.SetResult(sResult);
                        string trans = tempwt.GetTranslatedText();
                        
                        wTable.transWord = trans + "に" ;
                        wTable.Cost = 10;
                        wTable.posCategory  = PosCategory.PP  ;
                        
                        WordChain wc = new  WordChain(wTable);
                        
                        // これだけならwtでもいいが、汎用化のためwcで返す
                        return wc;
                    }
                }
            }
            
            return null;
        }
        //--------------------------------------------------------------------
        // 独立している１字の語は先にチェック
        //   （あとでhashか何かに変更）
        static private void  CheckOneWord(WordTable wt){
                
                if(KJ_dict.inputIsHangul){
                    // K-->J
                    if(wt.word == "그"){ 
                        wt.transWord = "その"; 
                        wt.posCategory = PosCategory.AdnominalDemonstrative;
                        wt.Cost = 0;
                        return; 
                    }   
                    
                    if(wt.word == "이"){ 
                        wt.transWord = "この"; 
                        wt.posCategory = PosCategory.AdnominalDemonstrative;
                        wt.Cost = 0;
                        return; 
                    }   
                    if(wt.word == "고"){ 
                        wt.transWord = "と"; 
                        wt.posCategory = PosCategory.PP;
                        wt.Cost = 0;
                        return; 
                    }   
                    if(wt.word == "며"){ 
                        wt.transWord = "とし"; 
                        wt.posCategory = PosCategory.PP;
                        wt.Cost = 0;
                        return; 
                    }   

                    if(wt.word == "가"){ 
                        if(wt.prev != null && wt.prev.IsWord() ){
                            //  TV가 --> TVが
                            wt.transWord = "が"; 
                            wt.posCategory = PosCategory.PP;
                            wt.Cost = 0;
                            return; 
                        }   
                    }
                }else{
                    // J-->K
                    // nop
                }
                
                return;
        }
            
        
    } // end of class

} // End namespace KJ_dict
