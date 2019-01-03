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

namespace HYAM.KJ_dict
{

    // 文字分割情報
    public enum Divided { 
        Non,            // 分割なし           AAABBB    KKKMMNNN
        Lead,           // 分割あり先頭部     AAA       KKK
        Middle,         //    〃   中間                 MM
        Trail           //    〃   末尾       BBB       NNN
    } ;
    

    public class WordTable {
        public  WordTable prev;       
        public  WordTable next;       
        public  string word;        // original word
        public  string transWord;   // translated word
                // sResultより優先
                
        public  CharCategory  charCategory;     // Character Category
        public  PosCategory   posCategory;      // part of speech
        
        public  Divided   divided;    // 分割情報

        public   SearchResult sResult;  // 辞書の検索結果チェーン
                                         // 複数の結果をArrayListで保持
                                         
        private  int   cost;   // transWordのcost
        

        //-----------------------------------------------------------
        // constructor
        public WordTable() {
            this.prev = null;
            this.next = null;
            this.word = "";
            this.transWord = "";
            this.charCategory   = CharCategory.Null ;
            this.posCategory  = PosCategory.Null ;
            this.sResult = null;
            this.divided = Divided.Non;
            this.cost = 100;  // 未知語
        }
        public WordTable( string input) : this() {
            this.word = input;
        }
        public WordTable( string input, string trans) : this() {
            this.word = input;
            this.transWord = trans;
        }

        //------------------------------------------------
        // property
        
        // 文頭の語ならtrue
        public bool IsSentenseHead {
            get{
                // その前がnullなら文頭
                if(this.prev == null) {
                    return true;
                }

                if(KJ_dict.inputIsHangul){ 
                // 入力ハングル

                    // その前が"." または 空白が後続する"." なら文頭
                    if(this.prev!=null && 
                        (this.prev.word=="."  ||
                          (Char.IsSeparator(this.prev.word[0]) &&
                           this.prev.prev!=null && this.prev.prev.word==".")
                        )
                      )
                    {
                             return true;
                         
                    }
                }else{
                    // 入力 日本語
                    if(this.prev!=null && this.prev.word=="。" ){
                        return true;
                    }
                }
                
                return false;
            }
        }
        
        // 文末の語ならtrue
        public bool IsSentenseTail {
            get{
                 if(KJ_dict.inputIsHangul){ 
                     // 入力ハングル
                     if(this.next!=null && this.next.word=="." &&
                        // 終端か、空白が後続する"."  なら文末
                        (  this.next.next==null || 
                          (this.next.next!=null && 
                           Char.IsSeparator(this.next.next.word[0])
                          ) 
                        ) 
                       )
                     {
                         return true;
                     }else{
                         return false;
                     }
                 }else{
                     // 入力 日本語
                     if(this.next!=null && this.next.word=="。" ){
                         return true;
                     }else{
                         return false;
                     }
                 }
               }
          //set{ is_tail=value; }
        }
        

        public int Cost {
            get{ return cost; }
            set{ cost = value; }
        }
        
        
        
        // method ---------------------------------------------
        //    設定時はcostを設定するのでmethodに
        public void SetResult(SearchResult result) {
            this.sResult = result;
            this.cost = this.GetWordCost();
        }

        //---------------------------------------------
        // browserの時は、タグ、英数字、記号はskip で使う
        public bool IsTransSkipWord() {
            if(this.charCategory == CharCategory.HTMLtag    || 
               this.charCategory == CharCategory.Letter     ||
               this.charCategory == CharCategory.Other      || 
               this.charCategory == CharCategory.Control    || 
               this.charCategory == CharCategory.Separator  || 
               this.charCategory == CharCategory.Digit         ){
                return true; // skipする
            }       
            return false; // skipしないする
        }
        //---------------------------------------------
        // 少しゆるいskip (タグ、記号はskip）
        //   英数字で始まる熟語があるので熟語を調べる際はこちらのskip
        public bool IsTransSkipWord2() {
            if(this.charCategory == CharCategory.HTMLtag    ||
               this.charCategory == CharCategory.Control    || 
               this.charCategory == CharCategory.Separator       ){
                return true; // skipする
            }       
            return false; // skipしないする 
        }

        //---------------------------------------------
        //  sdataにあったposのstringをPosCategoryに変換する。
        public PosCategory String2PosCategory(string pos) {
            
            if(pos.StartsWith("pn.")){
                return PosCategory.ProperNoun;
            }
            if(pos.StartsWith("v.")){
                return PosCategory.Verb;
            }
            if(pos.StartsWith("adj.") || pos == "adj"){
                return PosCategory.Adjective;
            }
            if(pos.StartsWith("adjectivalverb")){
                return PosCategory.AdjectivalVerb;
            }
            if(pos.StartsWith("adverb")){
                return PosCategory.Adverb;
            }
            if(pos.StartsWith("noun")){
                return PosCategory.Noun;
            }
            
            return PosCategory.Other;
        }
        //---------------------------------------------
        //  次の有効な語を返す。(空白skip)
        public WordTable NextWord() {
            WordTable wt = this.next;
            
            while(true){
                if(wt == null){  break ; }
                
                if(wt.IsWord()){
                    return wt;
                } 
                wt = wt.next;
            }   
            return null;
        }
        
        //  前の有効な語を返す。(空白skip)
        public WordTable PrevWord() {
            WordTable wt = this.prev;
            
            while(true){
                if(wt == null){  break ; }
                
                if(wt.IsWord()){
                    return wt;
                } 
                wt = wt.prev;
            }   
            return null;
        }
        
                
        //--語のチェック系メソッド-------------------------------------------
        //
        //  有効な語であるか？ 
        public bool IsWord() {
            if(this.charCategory == CharCategory.Separator || 
               this.charCategory == CharCategory.Control   ||
               this.charCategory == CharCategory.HTMLtag        ){
                // (空白や制御文字およびHTMLのタグなら偽)
                return false;
            } 
            return true;
        }
        //  数字であるか？ 
        public bool IsDigit() {
            if(this.charCategory == CharCategory.Digit ){
                return true;
            } 
            return false;
        }
        // その他 記号類
        public bool IsOther() {
            if(this.charCategory == CharCategory.Other ||
               this.charCategory == CharCategory.Punctuation   ){
                return true;
            } 
            return false;
        }

        //  翻訳済みであるか？ 
        public bool IsTranslated() {
            if(this.transWord == "" && this.sResult == null ){
                return false;  // 未翻訳
            }else{ 
                return true;   // 翻訳済み
            }
        }

        //------------------------------------------------------------------
        public int GetWordCost()
        {
            if(this.sResult == null){
                if(this.cost==100){
                    return  (this.cost * this.word.Length) ;  // 未知語は掛け算
                }
                return this.cost ;
            }
            if(this.cost == 9999 ){
                // costが9999ならdefault値を返す。
                return 100;
            }
       
     // 途中でSearchResultの状態が変わる可能性あり。よって削除。       
     //       if(this.cost != 100 ){
     //           //  costが100以外ならdefault以外が設定されている。
     //           //  ので、それを返す。
     //           return this.cost;
     //       }
            
            int minCost=9999;
            foreach ( DocumentData sdata in this.sResult.documents  ){
                string wcostString = sdata.GetCost(KJ_dict.inputIsHangul);
                if(wcostString==""){
                    wcostString="100";
                }
                
                int wCost = System.Convert.ToInt32(wcostString);
                if(wCost==9999){
                    // word that cannot be used
                    continue;
                }

                if(wCost==100){
                    wCost = wCost * this.word.Length;  // 未知語は掛け算
                }else {
                    wCost = wCost - this.word.Length;  
                    // 長い訳語ほどコストを下げるため引き算
                }
                
                if(minCost > wCost){
                    minCost = wCost; // 一番小さいcostを返す
                }
            }
            
            // 次回アクセス用
            this.cost = minCost;  
            
            return minCost;
        }
        
        //-----------------------------------------------------------------------
        // WordTableについている翻訳結果チェーン(sResult)から文字列を作る
        public string GetTranslatedText()
        {
            if(this.transWord != ""){
                // transWordが入っていたらそちらを優先する
                return this.transWord;
            }
            
            if(this.sResult == null){
                return "";
            }
            
            // 文中の位置などから不要な結果を除いて，
            // 除外が発生した場合はSearchResultを作り直す。
            this.ModifySearchResult();
            
            return  this.sResult.MakeTranslatedText();

        } // end of GetTranslatedText

        //-----------------------------------------------------------------------
        // 文中の位置などから不要な結果を除いて，
        // 除外が発生した場合はSearchResultを作り直す。
        private void ModifySearchResult()
        {
         //   if(this.prev==null || 
         //      (this.prev!=null && !this.prev.IsWord())
         //     )
         //   { 
         //     // 前の語は無い、または 空白など区切り文字
         //     //   wtは単語の先頭である
         //     // 語頭に来ない訳語は落とす。
         //       if(this.s_result != null){
         //           this.s_result.ExceptHeadWord();
         //       }
         //   }


            if(this.divided != Divided.Non) {
                // 単語の分割が発生している場合。
                
                if(this.divided == Divided.Lead){
                    // 部分語の先頭語にならないものは除外
                    this.sResult = SearchResult.ExceptHeadWord(this.sResult);
                }

                if(this.divided == Divided.Trail){
                    // 部分語の末尾語にならないものは除外
                    this.sResult = SearchResult.CheckTrail(this.sResult);
                }

             //   連体形は部分語には使わないなど
             //    (첩보원들은 の들은は連体形でない )
             //    (連体形なら  들은 xxxx の形だろう (들은は単独語であるべき) )
             //   if(this.sResult != null){
             //       this.sResult = SearchResult.CheckDividedWord(this.sResult);
             //   }
             // 2006.01.06 commented
            }else{
                // 語分割がない場合
                
                if(this.sResult != null){
                    // 非部分語で使わない語は外す 
                    this.sResult = SearchResult.ExceptFullWord(this.sResult);
                }
                
                // 文章の先頭の語
           //     if(this.IsSentenseHead){
           //         this.sResult = SearchResult.ExceptSentenseHead(this.sResult);
           //     }
            }
            

        }
        
        
    }// End class WordTable

} // End namespace KJ_dict
