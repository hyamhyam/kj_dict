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
    // 品詞情報
    public enum PosCategory {
        // 正式な名ではないよ....
        Null,
        PP,   // 助詞

        Adjective       ,    // 形容詞
        Adverb          ,    // 副詞
        
        AdjectivalVerb  ,    // 形容動詞
        
        Verb ,  // 動詞
        Numerative ,         // 助数詞     匹，台，個など
        Numeral ,            // 数詞       千  万   億
        NumerativePrefix,    // 冠数詞     第4回  の 第など
        Conjunction ,        // 接続詞     だから, または, ところで 
        AdnominalDemonstrative,  // 連体指示詞 （この、その）
        Noun,  // 名詞
        ProperNoun ,   // 固有名詞
        Mark,    //
        Idiom ,  // 熟語は特別扱い
        Other      
    } ;
    
    // 文字種情報
    public enum CharCategory { 
        Null, 
        Hiragana,    
        HiraganaWo, Katakana,   Kanji,  Hangul,  
        KanHira,            // 「走る」のように 「漢字＋ひらがな」
        Separator,          // 空白
        Control,            //  改行
        Digit,              // 数字
        Letter,             // 英字   "-"
        LetterDigit,        // 英字 + 数字
        LetterMix     ,     // 英字 + hangul/kanji
        Suffix ,   
        Punctuation ,
        Other     ,         //  " () % 
        HTMLtag             //  <br> <h1>  </h1> etc
    } ;
    

    //-------------------------------------------------------------------------
    // 一番最初の文字列分解
    public class TextScanner {
        
        String    inputString;  // 入力文字列
        int       stringIndex;  // 入力文字列の次回切り出し位置（文字数）

        private bool htmltext = false; // <xxxxx>をskipするか？
        
        // property
        public bool htmlText {
            //  true : HTMLをskipする
            get{ return htmltext; }
            set{ htmltext = value; }
        }

        
        
        public WordChain Scan( string inputstring  ){

            this.inputString   = inputstring;
            this.stringIndex   = 0;
            
            WordChain wChain = new WordChain() ;
            
            WordTable wordT;
            
            // inputStringから語のchainを作成する。
            while ( true )
            {
                // inputStringから１語切り出す
                wordT = PicupWordTable();
                if(wordT.word == ""){
                    break;  // 取り出せなかったら終了
                }

                wChain.Add(wordT);

            } // end of while ( PicupWordTable loop )


            // 漢字＋ひらがな などいくつかの並びはまとめる。 ( wordT merge loop )
            CharCategory  prevCharCategory = CharCategory.Null ;
            WordTable prevwordT = new WordTable();
            wordT = wChain.Head;
            while ( wordT != null  )
            {
                CharCategory mergedCharCategory;
                mergedCharCategory=IsMergedable(prevCharCategory, 
                                                wordT.charCategory ) ;
                
                if(mergedCharCategory!= CharCategory.Null) {
                   WordTable newWt = new WordTable(prevwordT.word + wordT.word);;
                   newWt.charCategory = mergedCharCategory ;
                   newWt.posCategory = PosCategory.Other;
                   
                   wChain.Delete(prevwordT);
                   wChain.Insert(wordT, newWt);
                   wChain.Delete(wordT);
                   
                   wordT = newWt;
                }
                
                prevCharCategory = wordT.charCategory ;

                prevwordT = wordT ;
                
                wordT = wordT.next ;  // 次の語へ

            } // end of while ( wordT merge loop )
                
            return (wChain);
        }
        

        //---------------------------------------------------------------------
        // 漢字＋ひらがな（ex. 走る）  英字＋数字（ex. MP3)は１つのテーブルに
        // まとめるための判定
        private CharCategory  IsMergedable(CharCategory prev, CharCategory here){
            
            if(prev==CharCategory.Kanji &&      //  漢字＋ひらがな（ex. 走る）
               here==CharCategory.Hiragana  ){
                return CharCategory.KanHira  ;
            }
            if(prev==CharCategory.Letter &&     // 英字＋数字（ex. MP3)
               here==CharCategory.Digit  ){
                return CharCategory.LetterDigit  ;
            }
            if(prev==CharCategory.Letter &&     // 英字＋漢字（ex. A社 )
               here==CharCategory.Kanji  ){
                return CharCategory.LetterMix  ;
            }
      //      if(prev==CharCategory.Letter &&     // 英字＋ハングル （ex. A사 )
      //         here==CharCategory.Hangul  ){
      //          return CharCategory.LetterMix  ;
      //      }
            
            return CharCategory.Null;
        }
        
        //--------------------------------------------------------------
        // 非常に簡易的な文字列分解。文字種が変わった所で切る。
        //   this.inputString から文字を取り出し
        //   語(と思われる)単位にまとめて，その語をWordTableで返す。   
        private WordTable  PicupWordTable (){
            
            StringBuilder rtn_str = new StringBuilder();
            rtn_str.Length = 0;

            bool inHTMLtag = false;  // Tagの中か否か？
            
            // 採取取り出し文字の文字種
            CharCategory  lastCharCategory   = CharCategory.Null ;
            
            // 取り出す語の文字種
            CharCategory  charCategory       = CharCategory.Null ;
            
            WordTable wordT = new WordTable(); 
            while ( true ) {
                 
                if(this.stringIndex >= this.inputString.Length){
                    break;
                }

                // 入力文字列のinstr_inex位置から １文字取り出し
                char ch =  this.inputString[this.stringIndex]  ;

                // 取り出した文字の種別を判定 
                lastCharCategory = GetCharCategory(ch);
                

                //  例えば <font size=+1>の "<"から ">"までを
                //  HTMLtagというカテゴリにまとめる。
                if(this.htmltext) {
                    if(ch == '<'){
                        inHTMLtag = true;
                    }
                    if(inHTMLtag ){
                        lastCharCategory = CharCategory.HTMLtag ;
                    }
                    if(ch == '>'){
                        inHTMLtag = false;
                    }
                }
                
                
                
                if(charCategory != CharCategory.Null ){  
                    if(charCategory != lastCharCategory){
                        break;   // 文字種が変わったら抜ける
                    }
                }else{
                    charCategory = lastCharCategory;
                }
                
                rtn_str.Append( ch.ToString() );
                
                this.stringIndex++; // 取り出し位置を進める
            } 
            
            wordT.word          = rtn_str.ToString();  // 文字列の設定
            wordT.charCategory  = charCategory ;       // 文字種別の設定
            

            if(charCategory == CharCategory.Katakana ){
                wordT.posCategory = PosCategory.Noun;
            }else if(charCategory == CharCategory.HiraganaWo ){
                wordT.posCategory = PosCategory.PP;
                wordT.Cost = 0;
            }else if(charCategory == CharCategory.Other ){
                wordT.posCategory = PosCategory.Other;
                wordT.Cost = 0;
            }else if(charCategory == CharCategory.Separator ){
                wordT.posCategory = PosCategory.Other;
                wordT.Cost = 0;
            }else{
                wordT.posCategory = PosCategory.Other;
            }
            
            return  wordT;
        }
        // (*1) this.htmltext
        //  true  : <br> は1個のwordTable
        //  false : <, br, > と3個のwordTableになる
        
        //----------------------------------------------------
        // 文字種を判定
        private CharCategory  GetCharCategory (Char ch){
            
                // 例外的な判定
                if(ch=='-'){
                    return CharCategory.Letter ;  // ハイフンは英字扱い
                }
                
                if( CodeCheck.IsHangul(ch) ){
                    return   CharCategory.Hangul  ;
                }else if(CodeCheck.IsKanji(ch)  ){
                    return   CharCategory.Kanji ;
                }else if(CodeCheck.IsKatakana(ch)  ){
                    return   CharCategory.Katakana ;
                }else if(CodeCheck.IsPunctuation(ch)  ){
                    return   CharCategory.Punctuation ;
                }else if(CodeCheck.IsHiragana(ch)  ){
                    if(ch=='を'){
                        return   CharCategory.HiraganaWo ; // "を"は単独で分離。特別扱い
                    }else{
                        return   CharCategory.Hiragana ;
                    }
                }else if(Char.IsSeparator(ch)  ){
                    return   CharCategory.Separator ;
                }else if(Char.IsControl(ch)  ){
                    return   CharCategory.Control ;
                }else if(Char.IsDigit(ch)){
                    return   CharCategory.Digit ;
                }else if(Char.IsLetter(ch)){
                    return   CharCategory.Letter ;
                }
                
                
                return  CharCategory.Other;
        }
        
    } // end of class KJ_TextScanner

} // End namespace KJ_dict
