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
    
    
        //------------------------------------------------------------------
    // 
    public class SearchResult{
        public int return_code;      //  0:normal end,
                                     //  -1:not found 
                                     //  1:found but 100 over
                                     //  -20 : regular expression error
                                     //  -21 : not support regular expression 
                                     
        public ArrayList documents;  // DocumentData型のArrayList
        
        
        //===========================================================================
        //
        //  static public methods
        //

        //------------------------------------------------------------------
        // 助詞に先行しない語は除く
        //     DocumentDataを作りなおすため、SearchResultも再作成
//        static public SearchResult CheckPPConnectable(SearchResult result) {
//            FilterDelegate transFilter = 
//                    new FilterDelegate( KJ_Filter.PPConnectable ) ;
//            return SearchResult.Except(result, transFilter) ;
//        }
//
        //------------------------------------------------------------------
        // 部分語には使わない訳語は外す
//        static public SearchResult CheckDividedWord(SearchResult result) {
//            FilterDelegate transFilter = 
//                    new FilterDelegate( KJ_Filter.NotDividedWord ) ;
//            return SearchResult.Except(result, transFilter) ;
//        }
        //     (*1) 대한간호협회 の대한は形容詞("対する")ではなく、"大韓"

        //------------------------------------------------------------------
        // 非部分語で使わない語は外す
        //     (*1) 간호협회 대한 の대한は"大韓"ではなく、"対する"
        static public SearchResult ExceptFullWord(SearchResult result) {
            FilterDelegate transFilter = 
                 new FilterDelegate( KJ_Filter.NotFullWord ) ;
            return SearchResult.Except(result, transFilter) ;
        }

        //------------------------------------------------------------------
        // 部分語の先頭語にならないものは除外。
        //     이오넥스는の 이は助詞ではない。
        static public SearchResult ExceptHeadWord(SearchResult result) {
            FilterDelegate transFilter = 
                new FilterDelegate( KJ_Filter.NotHeadWord ) ;
            return SearchResult.Except(result, transFilter) ;
        }

        //------------------------------------------------------------------
        // 部分語の最後にならないものは除外
        //     조사한の한は"ひとつの"ではない。"～した"である。
        static public SearchResult CheckTrail(SearchResult result) {
            FilterDelegate transFilter = 
                new FilterDelegate( KJ_Filter.NotTrail ) ;
            return SearchResult.Except(result, transFilter) ;
        }

        //------------------------------------------------------------------
        // 文章の先頭に来ないものは除外
        //   
        static public SearchResult ExceptSentenseHead(SearchResult result) {
            FilterDelegate transFilter = 
               new FilterDelegate( KJ_Filter.NotSentenseHead ) ;
            return SearchResult.Except(result, transFilter) ;
        }

        //------------------------------------------------------------------
//        static public SearchResult Trim(SearchResult result) {
//            // 翻訳系では有効でないDocumentを削除
//            FilterDelegate transFilter = 
//                      new FilterDelegate( KJ_Filter.Valid ) ;
//            return SearchResult.Except(result, transFilter) ;
//        }



        //=====================================================================
        //
        //  static private  methods
        //
        //------------------------------------------------------------------ 
        // 条件にあうものは除外(filterにかかるものを削除)
        //   作り直すとコストがかかるのでIsUsefulをfalseにするだけ
        static private SearchResult Except(SearchResult result, 
                                           FilterDelegate filter) {
            
            if(result==null || result.documents==null) {
                return result;  // 変更なし
            }
            
            // result.documentsから適切でない訳語を取り除く
            ArrayList documents = KJ_Extracter.Except(result.documents, filter) ;

            if(documents == result.documents){
                // 変更なし
                return result;
            }

            // 変更あり SearchResultを作り直す
            SearchResult newResult = new SearchResult();
            newResult.documents = documents ;
            
            if(newResult.documents.Count == 0) {
                newResult.return_code = -1 ;
            }else{
                newResult.return_code = 0 ;
            }
            
            return newResult ;

        }
        
        //=======================================================================
        //
        //  public methods
        //

        // newoneのデータは既にlist中に存在するか？ (重複検知を避けるため *1 )
        public bool IsExist( DocumentData newone ) {
            foreach ( DocumentData sdata in this.documents  ){
                if(sdata.dict_pointer == newone.dict_pointer){
                    return(true);
                }
            }
            return(false);
        }
        // (*1) 例えば aで部分一致をかける場合，Sambaは2回hitする。これを避ける。



        //------------------------------------------------------------------
        // 翻訳結果(sResult)から翻訳後文字列を作る
        // 複数マッチがあった場合は [aaa,bbb]のように列記
        // ただしコストが大きい語は除く
        //
        //    品詞の指定がある場合は、品詞で絞る
        //    
        public string MakeTranslatedText() {
            return MakeTranslatedText("");
        }
        
        // searchPosの指定がある場合は，documentsアレイから
        // 該当する品詞だけを取り出す
        public string MakeTranslatedText(string searchPos) {
            string trans_result = "" ; // 翻訳結果の返却値
            int hitcnt = 0;            // hit数


            int cost_min=9999;    // 検知したcostの最小値
            int cost;
            
            // 複数matchの可能性あり。その場合，まとめる。
            DocumentData min_sdata = null ; //最小コストのsdata

            foreach ( DocumentData sdata in this.documents  ){
                
                string pos = sdata.GetPos();
                if(searchPos!="" && pos!=searchPos){
                    continue;  // 非採用
                }
                

                string costString = sdata.GetCost(KJ_dict.inputIsHangul);
                if(costString==""){
                    cost = 100;
                }else{
                    cost = System.Convert.ToInt32(costString);
                }
                
                string key   = sdata.GetKey(KJ_dict.inputIsHangul);

//                // 訳語に ～および ... を含むものは自動翻訳では使わない。
//                //  （主に助詞を弾くため)
//                if(key.StartsWith("～") || key.EndsWith("～")  || 
//                   key.StartsWith("...") || key.EndsWith("...")  ){
//                    continue;  
//                }
//
//                // cost 9999は、無条件に使わない
//                if(cost == 9999 ){ 
//                    continue;  
//                }
                
                if(cost <  cost_min ){
                    // 今までのものより小さいcostが出たら初期化
                    // （costが一番小さい語を返すようにするため）
                    hitcnt=0;
                    trans_result="";
                    
                    // minを再設定
                    cost_min=cost;
                    
                    min_sdata=sdata; //最小コストのsdata
                    
                }else if(cost > cost_min ){
                    // 現在のものより大きいcostの場合、無視。
                    continue;  
                }
                if(trans_result != ""){
                    trans_result += ",";
                }
                trans_result += key;
                
                hitcnt++;
                
            }  // end of foreach  
            

            if(hitcnt > 1){  
                // 複数matchの同時表示
                trans_result = "[" + trans_result + "]" ;
            }

                
                //  String-->Enum変換必要
            //    if(min_sdata != null && this.posCategory == PosCategory.Null  ){
            //        this.posCategory = this.String2PosCategory(min_sdata.GetPos());
            //    }
                

            return(trans_result);
            
        } //  end of MakeTranslatedText
        
        

//        //----------------------------------------------------------------------
//        // 翻訳結果(sResult)から翻訳後文字列を作る
//        // 複数マッチがあった場合は [aaa,bbb]のように列記
//        //       品詞で絞る
//        public string GetResultwithPos(string searchPos) {
//            string trans_result = "" ; // 翻訳結果の返却値
//            int hitcnt = 0;            // hit数
//            
//            // 複数matchの可能性あり。その場合，まとめる。
//            foreach ( DocumentData sdata in this.documents  ){
//                
//                string pos = sdata.GetPos();
//                if(pos!=searchPos){
//                    continue;  // 非採用
//                }
//                
//                string key = sdata.GetKey(KJ_dict.inputIsHangul);
//                
//                if(trans_result != ""){
//                    trans_result += ",";
//                }
//                trans_result += key;
//                
//                hitcnt++;
//                
//            }  // end of foreach  
//            
//
//            if(hitcnt > 1){  
//                // 複数matchの同時表示
//                trans_result = "[" + trans_result + "]" ;
//            }
//                
//            return trans_result ;
//        }

        
    } // end of class SearchResult
 
 

} // End namespace KJdict

