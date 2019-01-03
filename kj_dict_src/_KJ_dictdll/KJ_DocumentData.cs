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
using System.Reflection;
using HYAM.Lingua ;


namespace HYAM.KJ_dict
{
    // 1個分のドキュメントデータ（検索データ）を保持するオブジェクト
    //     
    //     ---
    //     key1: 수풀         ---+
    //     key2: 林              |
    //     word2: はやし         | One DocumentData
    //     pronun1: SUPUL        |
    //     pronun2: HAYASI       |
    //     cost1: 17             |
    //     cost2: 17             |
    //     src: tachibana-u   ---+
    //     ---
    //     key1: 수프국물
    //     key2: スープ
    //     pronun1: SUPEUGUGMUL
    //     pronun2: SUPU
    //     cost1: 17
    //     cost2: 17
    //     src: tachibana-u
    //     ---
    
    
    
    //--------------------------------------------------------------------
    public class DocumentData{
        public Hashtable item;    // ドキュメントをhash化したもの
        public long dict_pointer; // このドキュメントの先頭位置
                                  //   (seek pointer)
                                  
                                  
        //------------------------------------------------
        // コンストラクタ
        public DocumentData(){  
            this.item = new Hashtable();
            this.dict_pointer = 0;
        }
        
        //------------------------------------------------
        // property
        
        // method for Part of Speach ---------------------

        //  動詞か？
        public bool IsVerb(){  
            string pos = GetPos();
            if(pos.StartsWith("v.") || pos == "v"){
                return true;
            }else{
                return false;
            }
        }

        //  助詞か？
        public bool IsPP(){  
            string pos = GetPos();
            if(pos.StartsWith("pp.")){
                return true;
            }else{
                return false;
            }
        }
        
        //  形容動詞か？
        public bool IsAdjectivalverb(){  
            string pos = GetPos();
            if(pos.StartsWith("adjectivalverb")){
                return true;
            }else{
                return false;
            }
        }

        //  形容詞か？
        public bool IsAdjective(){  
            string pos = GetPos();
            if(pos.StartsWith("adj.") || pos == "adj" ){
                return true;
            }else{
                return false;
            }
        }
        //  数詞か？
        public bool IsNumerativePrefix(){  
            string pos = GetPos();
            if(pos == "NumerativePrefix" ){
                return true;
            }else{
                return false;
            }
        }
        //-------------------------------

        public bool IsVerbAdnominal(){  
            string pos = GetPos();
            if(pos.IndexOf("adnominal") == -1 ){
                return false;
            }else{
                return true;
            }
        }
        public bool IsVerbPresent(){  
            string pos = GetPos();
            if(pos == "v.present" ){
                return true;
            }else{
                return false;
            }
        }
        public bool IsVerbRoot(){  
            string pos = GetPos();
            if(pos == "v.root" ){
                return true;
            }else{
                return false;
            }
        }
        // method for Part of Speach ---from here------------------
        

        //--------------------------------------------------------------------------
        public String GetData(string key){  
            if(this.item.ContainsKey(key)){
                return (string) this.item[key];
            }
            return "";
        }
        //------------------------------------------------------------
        public void SetData(string key, string data){ 
            if(this.item.ContainsKey(key)){
                this.item[key] = data ;       // 書き換え (cost再設定など)
            }else{
                this.item.Add(key, data);
            }
        }


        //------------------------------------------------------------
        // public method 
        public bool IsTailWord(){  
            // 原型、連体形は文章の語尾には使わない
            if(IsVerbRoot()  || IsVerbAdnominal() ) {
                return false;
            }
            return true;
        }
        
        public bool IsHeadWord(){  
            // "hyam(gobi)" の語は文章の先頭にはつけない
            string src = this.GetSrc();
            if(src == "hyam(gobi)" ) {
                return false;
            }
            return true;
        }
        
        public bool IsMiddleWord(){  
            if(IsVerbPresent()  ||  IsVerbRoot() ) {
                return false;
            }else{
                return true;
            }
        }
        
        
        //---------------------------------------------------------------------------
        // データ取得用

        public String GetCost(bool inputIsHangul ){  
            if(inputIsHangul ){
                return GetData("cost2");
            }else{
                return GetData("cost1");
            }
        }
        public String GetKey(bool inputIsHangul ){  
            if(inputIsHangul ){
                return GetData("key2");   // ひらがな，カタカナ，漢字
            }else{
                return GetData("key1");   // ハングル
            }
        }
        
        public String GetSrc(){  
            return GetData("src");
        }
        
        public String GetPos(){  
            return GetData("pos");
        }
        
                
    } // end of class DocumentData 
    

} // End namespace KJdict

