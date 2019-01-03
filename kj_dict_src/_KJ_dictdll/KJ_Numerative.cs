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
    class Numerative {
        
        //-------------------------------------------------------
        // 語のchainを先頭から舐め、助数詞・冠数詞を解析する。
        static public WordTable Scan ( WordChain wChain, WordTable wTable ){
            
            if(wTable.IsTranslated()){
                return wTable;  // 変換済みなら何もしない
            }

            WordTable next = wTable.NextWord() ; // 次の有効な語を返す。(空白skip)
            
            // 前後を数字(Digit)に挟まれる場合
            if((wTable.prev!=null && wTable.prev.IsDigit() ) &&
               (next!=null &&  next.IsDigit() )  ) {
                    string transword = KJ_Filter.SearchNumConnecting(wTable.word);
                    if(transword != ""){
                        wTable.transWord = transword;
                        wTable.posCategory = PosCategory.Numerative; 
                        wTable.Cost = 1;
                        return (wTable);
                    }
            }
            
            // 1つ前のWordTableは数字(Digit)の場合
            // または数詞(Numeral)である場合
            if((wTable.prev!=null && wTable.prev.IsDigit() ) ||    // 800억 の 억
               (wTable.PrevWord()!=null && 
                wTable.PrevWord().posCategory == PosCategory.Numeral)  // 800억원 --> 800+억+원の원
//                 160억 개의 のように空白をはさむ事があるので空白skipで前の語を調べる
              )
            {
                WordChain dividedChain = DivideCountable(wTable);
                
                if(dividedChain != null){
                    // 助数詞であった
                    
                    // devidedChainは 助数詞＋語のチェーン。wTableと入れ替える
                    //   （＋語はない場合もある）
                    wChain.Swap(wTable, dividedChain);
                    
                    // 処理済みTable変更
                    wTable=dividedChain.Head;  
                }
                
            }else{
                // 次の語が数字である場合 (冠数詞)  NumerativePrefix
                //   ★前数字と違い、空白はさんでも許す  (약 90만  or  약90만)
                if(next!=null &&  next.IsDigit() ){
                    string transword = KJ_Filter.SearchPreNum(wTable.word);
//                    string transword = CheckPreNum(wTable);
                    if(transword != ""){
                        wTable.transWord = transword;
                        wTable.posCategory  = PosCategory.NumerativePrefix; 
                                                     // ちょっと違うか。あとで
                        wTable.Cost = 1;
                    }
                }
            }
            
            return (wTable);
        }
        
        
        
        //-------------------------------------------------------
        static private WordChain DivideCountable(WordTable wTable ){
            // 数詞チェック
            WordChain wc = DivideCountableMain(wTable,   PosCategory.Numeral );
            
            if(wc==null){
                // 数詞でなかったら、助数詞チェック
                wc = DivideCountableMain(wTable,  PosCategory.Numerative );
            }

            return wc;
        }
        
        //-------------------------------------------------------
        static private WordChain DivideCountableMain(WordTable wTable, PosCategory poscategory ){
            WordChain wc=null;

            StringDivider sd = new  StringDivider(wTable.word);
            
            // 後から1文字ずつ落としつつ，検索。
            string trans = "";
            while( sd.eof() == false ) {
                // 文字列を分割する
                HYAM.KJ_dict.Pair pair = sd.DivideBackward() ;
                
                if(poscategory==PosCategory.Numeral){
                    trans = KJ_Filter.SearchNumeral(pair.head);
                }else{
                    trans = KJ_Filter.SearchNumerative(pair.head);
                }
                
                if(trans != ""){
                    
                    WordTable wt_num = new WordTable(pair.head, trans);
                    wt_num.posCategory = poscategory;
                    wc = new WordChain(wt_num);
                    
                    if(pair.tail != ""){
                        WordTable wt_tail = new WordTable(pair.tail);
                        wc.Add(wt_tail);
                    }
                    
                    return wc;
                }
                
            }  // end of while          
            
            
            return null ;
        }
        

        
    } // end of class KJ_Numerative

} // End namespace KJ_dict
