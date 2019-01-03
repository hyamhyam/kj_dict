// Copyright 2004, 2009 hyam <hyamhyam@gmail.com>
// and it is under GPL
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2, or (at your option)
// any later version.
 
using System;
using System.Collections;

using HYAM.Lingua;

namespace HYAM.KJ_dict
{



    //-----------------------------------------------------------------------------   
    // 条件付きサーチを行うメソッド群
    // 
    public class KJ_Filter {
        

        //===========================================================================
        //
        //  public methods
        //

        //-------------------------------------------------------------
        // 助詞系
        static public string SearchPPall (string inputword){
            string trans="";
            trans = SearchDictwithPos(inputword, "pp.pachim" ); ;
            if(trans != ""){
                return trans;
            }
            trans = SearchDictwithPos(inputword, "pp.nopachim" ); ;
            if(trans != ""){
                return trans;
            }
            trans = SearchDictwithPos(inputword, "pp.common" ); ;
            if(trans != ""){
                return trans;
            }
            return "";
        }
        static public string SearchPP_Pachim (string inputword){
            string trans="";
            trans = SearchDictwithPos(inputword, "pp.pachim" ); ;
            if(trans != ""){
                return trans;
            }
            trans = SearchDictwithPos(inputword, "pp.common" ); ;
            if(trans != ""){
                return trans;
            }
            return "";
        }
        static public string SearchPP_NoPachim (string inputword){
            string trans="";
            trans = SearchDictwithPos(inputword, "pp.nopachim" ); ;
            if(trans != ""){
                return trans;
            }
            trans = SearchDictwithPos(inputword, "pp.common" ); ;
            if(trans != ""){
                return trans;
            }
            return "";
        }
        
        
        //-------------------------------------------------------------
        // 数値系
        static public string SearchNumeral (string inputword){
            return   SearchDictwithPos(inputword, "Numeral" );
        }

        static public string SearchNumerative (string inputword){
            return   SearchDictwithPos(inputword, "Numerative" );
        }
        
        static public string SearchPreNum (string inputword){
            return   SearchDictwithPos(inputword, "NumerativePrefix" );
        }
                      
        static public string SearchNumConnecting (string inputword){
            return   SearchDictwithPos(inputword, "NumConnecting" );
        }
                      

        //-------------------------------------------------------------
        // 接頭語  接尾語
        static public string SearchPrefix (string inputword){
            return   SearchDictwithPos(inputword, "Prefix" );
        }
        static public string SearchSuffix (string inputword){
            return   SearchDictwithPos(inputword, "Suffix" );
        }

        //=====================================================================
        // 除外のためのフィルター
        //    return true は  Fileterを通したあと除外される
        //
//        public static bool PPConnectable( DocumentData doc )     { 
//            if(doc.IsVerbAdnominal()){
//                return false;
//            }else{
//                return true;
//            }
//        }
//

        //------------------------------------------------------------
        // 分割した場合の先頭に来ない語はtrue
        //    動詞,形容詞,形容動詞は部分語の先頭には来ない
        //    AABBCC のAAは助詞ではない。 etc
        public static bool NotHeadWord( DocumentData doc ) { 

            if(doc.IsPP()        //     || 
        //       doc.IsVerb()           ||
        //       doc.IsAdjectivalverb() || 
        //       doc.IsAdjective()            
        //  어렵다고 말했다.
        //  などあるので「動詞,形容詞,形容動詞」は除外
            ){
                return true;
            }
        
            return false;
        }
        
        
        //------------------------------------------------------------
        // 分割した場合の最後に来ない語はtrue
        //    AABBCC のCCに来ない語はture
        //    조사한の한は連体(ひとつの)ではない。  etc
        public static bool NotTrail( DocumentData doc ) { 

            string pos = doc.GetPos();
            if( pos == "adj.adnominal") 
            {
                return true;
            }

        
            return false;
        }
        
        //------------------------------------------------------------
        // 文章の先頭に来ないものはtrue
        public static bool NotSentenseHead( DocumentData doc ) { 

            string src = doc.GetSrc();
            if( src == "hyam(verb.hada.parts)") // 動詞の活用語尾
            {
                return true;
            }
        
            return false;
        }
        

        //------------------------------------------------------------
        // 分割語(DividedWord)にならないならtrue
        public static bool NotDividedWord( DocumentData doc )     { 
        

            // 動詞,形容詞,形容動詞は部分語の訳語とはしない
            if(doc.IsVerb() || doc.IsAdjective() || doc.IsAdjectivalverb()){
                
                // ただし長さ3以上は採用する。
                string key;
                if(KJ_dict.inputIsHangul){
                    key = doc.GetData("key1");
                }else{
                    key = doc.GetData("key2");
                }
                if(key.Length > 2){
                    return false;
                } 

                // ただし動詞で例外あり
                string root = doc.GetData("root1");
                if(root == "하다" ||      
                   root == "되다" || 
                   root == "시키다"     ) { // よくないけど、あとで考え直す
                    // nop    (*2)
                }else{
                    return true;
                }
            }
            return false;
        }
        
        //---------------------------------------------------------
        // 非分割の語で使わないものは外す
        public static bool NotFullWord( DocumentData doc )     { 
            if(doc.GetPos() == "noun.partial"){
                return true;
            }

          //  string src = doc.GetSrc();
          //  if( src == "hyam(verb.hada.parts)") // 動詞の活用語尾
          //  {
          //      return true;
          //  }
          //  표시하기도 했다 のように離れている語尾を翻訳できなくなる
          //  ので判定しない。
        
            return false;
        }
        
        //---------------------------------------------------------
        // まだ使ってない
        public static bool Valid( DocumentData doc )     { 
            string key   = doc.GetKey(KJ_dict.inputIsHangul);
            
            // 訳語に ～および ... を含むものは自動翻訳では使わない。
            // （主に助詞を弾く)
            if(key.StartsWith("～") || key.EndsWith("～")  || 
               key.StartsWith("...") || key.EndsWith("...")  ){
                return false;  
            }
        
            string costString = doc.GetCost(KJ_dict.inputIsHangul);
            // cost 9999は、無条件に使わない
            if(costString == "9999" ){ 
                return false;  
            }

            return true;
        }

        
        // (*2) xxxxする、xxxxさせる  etc
        //
        //======================================================================
        //
        //  private methods
        //
        //-------------------------------------------------------------
        
        // 品詞条件付きサーチ
        static private string SearchDictwithPos (string inputword, string pos){
            SearchResult  sResult = KJ_dict.SearchFull(inputword);
               
            if(sResult==null){
                return "";
            }
            
            return sResult.MakeTranslatedText(pos);
            
        }
        

    }  // End class KJ_Filter
                  

    //------------------------------------------------------------------------   
    // 
    // DocumentDataのArrayListから条件に合わない要素を捨てるためのclass
    // 
    public delegate bool FilterDelegate(DocumentData doc); // デリゲート型を定義

    class KJ_Extracter   {
        //
        //  DocumentDataのArrayListを入出力
        //
        
        // 条件にあうものは除外
        public static ArrayList Except ( ArrayList inList, FilterDelegate filter )
        { 
            ArrayList outList = new ArrayList();
            
            bool changed = false;

            foreach ( DocumentData doc in inList ) {
                
                if(filter(doc)){  
                    // filterにかかるものを削除
                    changed = true;
                }else{
                    outList.Add(doc);  
                }
            }
            
            if(changed){
                // 変更があった。
                return outList;
            }else{
                // 変更なしなら、入力をそのまま返す
                return inList ;
            }
        }

//        // 条件にあうものを選択
//        //   inListはDocumentDataのArrayList
//        public static ArrayList Select( ArrayList inList, FilterDelegate filter )
//        { 
//            ArrayList outList = new ArrayList();
//            
//            bool changed = false;
//            
//            foreach ( DocumentData doc in inList ) {
//                
//                if(filter(doc)){  // filterにかかるものを残す
//                    outList.Add(doc);  
//                }else{
//                    changed = true;
//                }
//            }
//            
//            if(changed){
//                // 変更があった。
//                return outList;
//            }else{
//                // 変更なしなら、入力をそのまま返す
//                return inList ;
//            }
//        }

    }  // end of class KJ_Extracter

   
} // end of Namespace
 