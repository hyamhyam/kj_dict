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
using System.Diagnostics ;

using HYAM.Lingua;

namespace HYAM.KJ_dict
{


    //------------------------------------------------------------------------
    //
    // 辞書本体とSuffix Array fileをまとめて辞書検索を操作するためのクラス
    //
    //   KJ_dict.yml＋KJ_dict.yml.w.aryのように辞書本体とarray indexをセットで
    //   オブジェクトとして持つ。
    //   arrayは文字か語単位かどちらか１つだけをもつ。
    //
    //   そのためKJ_formでは２つのオブジェクトが必要
    //   KJ_dictクラスの中に２つのオブジェクトを保持している。
    //
    public class KJ_DictData
    {
        // 辞書ファイル用
        private FileStream   dict_fs ;
        private StreamReader dict_reader;

        // Suffix Array用
        private FileStream   array_fs ;
        private BinaryReader array_reader;
        
        //検索に使うarrayのindexの最大値
        private long   array_index_max;  
        
        // 検索語と2分検索中の語のマッチング時判定用
        private  KJ_dictword   dictword; 
        
        
        private FilterDelegate dictFilter;  
        // 辞書の検索データのフィルタ（不要なデータは返さない）

        private FilterDelegate allvalid;    
        // 全て通すフィルタ (切り替え時にそなえ保管)
        
        
        //-------------------------------------------------------
        // constructor
        public KJ_DictData (string yamlfile, string arrayfile ) {

            // 辞書本体のOpen
            this.dict_fs = new FileStream(yamlfile, FileMode.Open, FileAccess.Read);
            this.dict_reader = new StreamReader(this.dict_fs, Encoding.UTF8 );
            
            // 途中で辞書ファイルが変わることがないのでstaticに設定しておく
      //      KJ_SuffixArray.dict_fs     = this.dict_fs ;
      //      KJ_SuffixArray.dict_reader = this.dict_reader ;
             // StreamReaderにSeekは効かないので使えない
             
            System.IO.FileInfo fi;

            // Array fileのOpen
            fi = new System.IO.FileInfo(arrayfile);
            array_index_max = (fi.Length / 4) - 1 ;
            array_fs  = new FileStream(arrayfile, FileMode.Open, FileAccess.Read);
            array_reader = new BinaryReader(array_fs );
            
            // 判定用object設定
            //   文字のマッチングルール切り替えを前提に別class
            dictword = new KJ_dictword();
            
            // デフォルトのフィルタを設定
            dictFilter = new FilterDelegate(this.allValid);
            allvalid = dictFilter; // 書き換えの保管用 
        }

        //---------------------------------------------------------------------
        //    辞書引きのメインのサーチメソッド。
        public SearchResult Search( SearchType searchtype , String search_word ){
            
            
            SearchResult result = new  SearchResult();
            
            #if DEBUG_DICTLOG
            KJ_dict.WriteDebugLog
                ("------------------------------------------------------------");
            KJ_dict.WriteDebugLog("Search Start");
            #endif           

            // 条件にマッチするarrayの要素の先頭番号を調べる
            long match_top = bsearch_top(searchtype, search_word);
        
            #if DEBUG_DICTLOG
            KJ_dict.WriteDebugLog("Search End");
            #endif           
         
            if( match_top == -1 ) 
            {
                result.return_code = -1;
                result.documents = new ArrayList() ;
                return( result );   // 検索ができなかった
            }
        
            // マッチした先頭から順に取り出し，resultにつなぐ
            result.return_code = 0;
            result.documents = new ArrayList();
    
            long index = match_top ;
            int cnt=0;
            while(true)
            {
                // suffix arrayのindex番目の語を取り出す
                String word = 
                    KJ_SuffixArray.GetWord(this.dict_fs, this.array_reader, index);
                
                // 取り出した語は検索条件にマッチするか？
                if(this.dictword.Compare(searchtype, search_word, word)) 
                {
                    // マッチするならその語を含むドキュメント全体を取り出す
                    long dict_pointer = 
                        KJ_SuffixArray.GetSeekPointer(this.array_reader, index) ;
                    DocumentData docdata = 
                        KJ_yaml.GetBlock(this.dict_fs, dict_pointer);
                    
                    // 既出でないならresultに追加
                    if(!result.IsExist(docdata)){
                        
                        // 絞込みのフィルタ
                        if(this.dictFilter(docdata)){
                            result.documents.Add( docdata );  // 戻り値として追加
                        }else{
                            // フィルタにマッチしないなら返さない
                            cnt--;   // adhocだが、先に1減じておく
                        }
                      
                    }else{
                        // 既出ならcntしない
                        cnt--;   // adhocだが、先に1減じておく
                    }
                }
                else
                {
                    break;    //   一致しなければ終了
                }
                index++;  // suffix arrayの次へ
                if(index > array_index_max )
                {
                    break;
                }
            
                // 検索語の数をカウント
                cnt++;
                
                if(cnt > 100)
                {
                    result.return_code = 1; // 100個を越えたら中断
                    break;
                }
            }
        
            // 返すべき値がなければ戻り値を -1に
            if(cnt==0){
                result.return_code = -1;
            }
            
            // 検索結果を返す
            return(result);
        }
    

        //---------------------------------------------------------------------
        // cost:8888を表示するしないを切り替えるためのフィルタ
        public void SetFilter( FilterDelegate dictFilter){
            this.dictFilter = dictFilter ;
        }
        public FilterDelegate GetFilter(){
            return this.dictFilter ;
        }
        
        // 全trueに戻す
        public void Set_allValid( ){
            this.dictFilter = this.allvalid ;
        }
        
        //===========================================================================
        //
        //  private methods
        //
        
        // デフォルトのフィルタ 
        private bool allValid(DocumentData doc){
            
            //実験用の動詞以外を表示するフィルタ
            //if(doc.IsVerb()){
            //  return false;
            //}else{
            //    return true;
            //}
            
            return true;
        }
        //---------------------------------------------------------------------
        // 条件にマッチする辞書の登録語の最初のグループを調べる
        private long bsearch_top(SearchType searchtype , String search_word) 
        {
            
            if(search_word.Length == 0){
                return(-1);   // 該当語なし
            }

            // とりあえずマッチする１個を調べる
            long index = bsearch_match_one(searchtype, search_word);
            if( index == -1 ) 
            {
#if DEBUG_DICTLOG
                KJ_dict.WriteDebugLog("■Cannot found [" + search_word + "]");
#endif 
                return(index);   // 該当語なし
            }
        
            // indexから前方にマッチする間さかのぼる
            int cnt=0;  // カウンタ
            while(true)
            {
                if(cnt > 100){
                    // 100以上は さかのぼらない
                    //   最大100個しか返さないので無駄な検索はしない
                    break;
                }
                cnt++;

                // indexは一致している。その１つ前を調べる。
                long check = index - 1 ;
                if(check < 0 )
                {
                    break;  // indexは0が最小。
                }
                String word = KJ_SuffixArray.GetWord(this.dict_fs, 
                                                     this.array_reader, check);
                if(this.dictword.Compare(searchtype, search_word, word)) 
                {
                    // １つ前も一致したならさらにさかのぼる
                    index = check;
                }
                else
                {
                    break;  // 一致しないなら終了
                }
            }
        
            return(index);
        }

        //---------------------------------------------------------------------
        // とりあえずマッチする１個を調べる     （バイナリサーチ）
        //   マッチする一群の先頭とは限らない
        //
        // もし見つかったらSuffix Arrayのindex pointを返す
        // -1  : 見つからなかった
        //
        public long bsearch_match_one(SearchType searchtype , string search_word) 
        {
            long low  ;
            long high ; 

            high = array_index_max ;
            low  = 0;
        
            long index;

            #if DEBUG_DICTLOG
            KJ_dict.WriteDebugLog("index,low,high");
            #endif            

            //  最初の検索index設定
            index = ( high - low ) / 2 + low ;  //  5/2-->2 (.5切捨て)
            while(true)
            {
                String word = KJ_SuffixArray.GetWord(this.dict_fs, 
                                                     this.array_reader, index);
            
                #if DEBUG_DICTLOG
                KJ_dict.WriteDebugLog(index + "," + low + "," + high);
                KJ_dict.WriteDebugLog("  word=" + word + "(" +
                                      index  + ")"  );
                String wordTemp;
                wordTemp = KJ_SuffixArray.GetWord(this.dict_fs, 
                                                     this.array_reader, low);
                KJ_dict.WriteDebugLog("  low =" + wordTemp + "(" +
                                      low  + ")"  );
                wordTemp = KJ_SuffixArray.GetWord(this.dict_fs, 
                                                     this.array_reader, high);
                KJ_dict.WriteDebugLog("  high=" + wordTemp + "(" +
                                      high  + ")"  );
                #endif            
            
                if(this.dictword.Compare(searchtype, search_word, word)) 
                {
                    // 見つかったので抜ける
                    #if DEBUG_DICTLOG
                    KJ_dict.WriteDebugLog("■Found ["  + search_word +
                                          "] (index=" + index + 
                                          " word="   + word  + ")" );
                    #endif 
                    break;
                }

                if(low == index)
                {
                    if(high == (low + 1 ) )
                    {
                        // もしlowとhighが1しか違わないなら
                        // lowはチェック済みなのでhighをチェックすればいい
                        index = high; //  次の検索index再設定
                        continue;
                    }
                    else
                    {
                        return(-1);   // マッチせず
                    }
                }
                if(high == index)
                {
                    return(-1);   // マッチせず
                }

                string  word_no_space = word.Replace(" ", "");                      // 2005.08.19
                string  search_word_no_space = search_word.Replace(" ", "");        // 2005.08.19
                if(String.CompareOrdinal(word_no_space, search_word_no_space) < 0 ) // 2005.08.19
       //         if(String.CompareOrdinal(word, search_word) < 0 ) // 2005.08.19
                {
                    low  = index;
                    // もしlowとhighが1しか違わないなら
                    // lowはチェック済みなのでhighをチェックすればいい
                    if(high == (low + 1 ) )
                    {
                        index = high; //  次の検索index再設定
                        continue;
                    }
                }
                else
                {
                    high = index;
                }
                index = ( high - low ) / 2 + low ;  //  次の検索index再設定

            }
        
            return(index);
        }
        


    } // End class KJ_DictData


    //-------------------------------------------------------------------------
    // KJ_dictword
    public class KJ_dictword
    {
        //---------------------------------------------------------------------
        // 検索語と2分検索中の語のマッチング
        public bool Compare(SearchType searchType, String search_word, String dict_word)
        {
            string  dict_word_no_space = dict_word.Replace(" ", "");      // 2005.08.19
            string  search_word_no_space = search_word.Replace(" ", "");  // 2005.08.19

            if( searchType == SearchType.full || searchType == SearchType.backward )
            {
                // 完全一致
                if(String.CompareOrdinal(dict_word_no_space, search_word_no_space) == 0 ) // 2005.08.19
//                if(String.CompareOrdinal(dict_word, search_word) == 0 ) 
                {
                    return(true);    // 一致した
                }
                else
                {
                    return(false);
                }
            }
            else
            {
                // 前方一致
                return(dict_word_no_space.StartsWith(search_word_no_space)) ; // このメソッドは元々bool型
//                return(dict_word.StartsWith(search_word)) ; // このメソッドは元々bool型  // 2005.08.19
            }
        
        }
    
    } // End class KJ_dictword



} // End namespace HYAM.KJ_dict
