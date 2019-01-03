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
using System.Text.RegularExpressions;

using HYAM.Lingua;


namespace HYAM.KJ_dict
{
    

    public enum SearchType { full,  forward,  backward,  part, regex };

    //------------------------------------------------------------------------
    //
    // 辞書引きのメインクラス
    //
    public class KJ_dict
    {
        static public KJ_DictData  dict_instance_w;
        static public KJ_DictData  dict_instance_c;

        // 検索方向。（設定必須ではない）
        static private bool input_is_hangul;   

        // 正規表現検索のためのfiltering word
    //    static private string filterHeadword;  
    //    static private string filterTailword;  
        static private string filterRegexword;  
        

        // 検索結果を記憶するHashを使うか？
        static private bool hash_available;
        
        // 検索結果を記憶するHash
        static private Hashtable dictHash; //完全一致辞書引きの記憶
        static private Hashtable dictCheckForwardHash;  // 前方一致存在チェック用
        static private Hashtable dictCheckFullHash;     // 完全一致存在チェック用
        
        
        #if DEBUG_DICTLOG
        // logファイル用
        static private FileStream   log_fs ;
        static private StreamWriter log_w;
        #endif
    
        // public property ---------------------------------------------------
        static public bool inputIsHangul {
            get{ return input_is_hangul; }
            set{ input_is_hangul = value; }
        }
        static public bool HashAvailable {
            get{ return hash_available; }
            set{ hash_available = value; }
        }
        
        //---------------------------------------------------------------------
        // static コンストラクタ
        //
        static  KJ_dict()
        {
            #if DEBUG_DICTLOG
            KJ_dict.StartDebugLog();
            KJ_dict.WriteDebugLog("KJ_dict static constructor");
            #endif
            
            KJ_dict.HashAvailable = false;             // デフォルトはHash未使用
            KJ_dict.dictHash = new Hashtable();   // hash初期化
            KJ_dict.dictCheckForwardHash = new Hashtable();  // hash初期化
            KJ_dict.dictCheckFullHash    = new Hashtable();  // hash初期化
            
        }
    
        //-------------------------------------------------------
        #if DEBUG_DICTLOG
        static public void StartDebugLog () {
            // logのOpen
            if(log_fs==null){
#if EDICT 
                log_fs  = new FileStream("Log_Edict.txt", 
#else
                log_fs  = new FileStream("Log_dict.txt", 
#endif
                                         FileMode.Create, 
                                         FileAccess.Write);
                log_w   = new StreamWriter(log_fs, Encoding.UTF8);
            }
        }
        //-------------------------------------------------------
        static public void WriteDebugLog (string logText) {
            if(log_w!=null){
                log_w.Write (logText + "\n" );
               // log_w.Write ("## " + logText + "\n" );
                log_w.Flush();
            }
        }
        //-------------------------------------------------------
        static public void EndDebugLog () {
            if(log_w!=null){
                log_w.Close();
                log_fs=null;
            }
        }
        #endif            

        //===========================================================================
        //
        //  public methods
        //
        //---------------------------------------------------------------------
        // 辞書ファイルOpen 
        //     default     KJ_dict.ymlのarrayはKJ_dict.yml.w.ary/KJ_dict.yml.c.ary
        static public void  DictOpen(String  yaml_master){
            DictOpen(yaml_master, yaml_master);
        }
        // 辞書ファイルOpen 
        //     Arrayファイルを個別に指定する場合
        static public void  DictOpen(string  yaml_master, string arrayfile){
            
            yaml_master = "dict\\" + yaml_master;
            arrayfile   = "dict\\" + arrayfile;
            dict_instance_w = new KJ_DictData(yaml_master, (arrayfile + ".w.ary") );
            dict_instance_c = new KJ_DictData(yaml_master, (arrayfile + ".c.ary") );
            #if DEBUG_DICTLOG
            KJ_dict.WriteDebugLog("## DictOpen ##"); 
            KJ_dict.WriteDebugLog("yaml_master=" + yaml_master + 
                                  ", arrayfile=" + arrayfile );
            #endif
        }
        
        
        // フィルタ切り替えのラッパ
        static public void SetFilter(FilterDelegate dictFilter){
            dict_instance_w.SetFilter(dictFilter);
            dict_instance_c.SetFilter(dictFilter);
        }
        static public FilterDelegate GetFilter(){
            return dict_instance_w.GetFilter();
        }
        static public void Set_allValid(){
            dict_instance_w.Set_allValid();
            dict_instance_c.Set_allValid();
        }
        
        
        //---------------------------------------------------------------------
        // 辞書引きのラッパー
        //
        
        //     KJ_form.exeで使う   defaultのindex arrayを使用する
        static public SearchResult  DictSearch(SearchType type , String searchword){
            
        //    if(type == SearchType.regex){
        //        
        //        int firstAster = searchword.IndexOf("*") ;
        //        int lastAster  = searchword.LastIndexOf("*") ;
        //        char[] any = new char[]{'?', '+', '[', ']', '{', '}' } ; 
        //        if(firstAster>=0 && firstAster==lastAster && 
        //           searchword.IndexOfAny(any)<0  ){
        //            // "*"が1個だけ
        //            return AsterRegexSearch(searchword) ;
        //        }else{
        //            return RegexSearch(searchword) ;
        //        }
        //    }
            
            // 検索タイプから使用するarrayを決定
            if( type == SearchType.full || type == SearchType.forward )
            {
                return  dict_instance_w.Search( type, searchword);
            }else{
                return  dict_instance_c.Search( type, searchword);
            }
        }

        //---------------------------------------------------------------------
        // 完全一致で辞書の存在チェック
        //     KJ_dict.Checkのラッパー。 
        //     翻訳時(KJ_form2.exe, KJ_browse.exe)のみ使う 
        static public bool  CheckFull(String str){
            return  ExistenceCheck( SearchType.full, str);
        }
        //---------------------------------------------------------------------
        // 前方一致で辞書の存在チェック
        //     KJ_dict.Checkのラッパー。 
        //     翻訳時(KJ_form2.exe, KJ_browse.exe)のみ使う 
        static public bool  CheckForward(String str){
            return  ExistenceCheck( SearchType.forward, str);
        }
        
        //---------------------------------------------------------------------
        // 完全一致で辞書を引く   
        //     hashがあればそちら優先
        //
        //     dict_small_w.Searchのラッパー。 
        //     KJ_trans(KJ_form2/KJ_browser)用
        //
        static public SearchResult  SearchFull(String str){
            
            if(inputIsHangul ){
                // K-->Jモードの時はひらがなカタカナ漢字はそのまま返す。
                //   KJ_form2/KJ_browserで韓日翻訳方向でひらがなをハングル化しないように
                if(CodeCheck.IsHiragana(str)  ||
                   CodeCheck.IsKatakana(str)  ||  
                   CodeCheck.IsKanji(str)           ){   
                    return(null);   // そのまま返す
                }
            }else{
                if( CodeCheck.IsHangul(str) ){
                    return(null);   // そのまま返す
                }
            }
            
            
            if(KJ_dict.HashAvailable){
                // Hashを使用する場合
                SearchResult hashedResult = (SearchResult)KJ_dict.dictHash[str];
                if(hashedResult == null){
                    // nop 
                }else{
                    // 既に辞書引き済み
                    if(hashedResult.return_code == -1){
                        return null;
                    }else{
                        return hashedResult;
                    }
                }
            }
            
            // 本当の辞書引き
            SearchResult result = dict_instance_w.Search(SearchType.full, str);
            
            if(KJ_dict.HashAvailable){
                // Hashを使用する場合, 結果を記憶する
                KJ_dict.dictHash[str] = result;
            }
            
            if(result.return_code == -1 ){
                return null;   // not found
            }
            
            // 完全一致で見つかった
            return result ;  // 検索結果を返す
        }   



        //===========================================================================
        //
        //  private methods
        //
    
        //---------------------------------------------------------------------
        // １つでも条件にマッチする語が存在するかのチェックだけ。 
        //    単なる存在チェック用
        //    hashがあるときはそちらを使う。
        //
        //    KJ_trans(KJ_form2/KJ_browser)用
        //
        static private bool ExistenceCheck( SearchType type ,  String search_word  )
        {

            if(KJ_dict.HashAvailable){
                // Hashを使用する場合
                if(type == SearchType.forward){
                    if(KJ_dict.dictCheckForwardHash.ContainsKey(search_word)){
                        // キーが存在する。
                        return (bool) KJ_dict.dictCheckForwardHash[search_word];
                    }else{
                        // 存在しない。
                        // nop
                    }
                }else if(type == SearchType.full){
                    // 完全一致でチェックする場合
                    SearchResult hashedResult =
                       (SearchResult)KJ_dict.dictHash[search_word];
                    if(hashedResult == null){
                        // 辞書引きなし。チェック済みかを調べる
                        if(KJ_dict.dictCheckFullHash.ContainsKey(search_word)){
                            // チェック済みなら前回の結果を返す
                            return (bool) KJ_dict.dictCheckFullHash[search_word];
                        }else{
                            // nop
                        }
                        
                    }else{
                        // 辞書引き済み
                        if(hashedResult.return_code == -1){
                            return false;
                        }else{
                            return true;
                        }
                    }
                }else{
                    // チェックは今のところ, fullとforwardだけ
                    Debug.Assert(false) ;
                }
            }
            
            // hashを使わない場合、またはsearch_wordが未チェックの場合
            
            // マッチするものが存在するかどうか１個だけ調べる
            // 検索タイプから使用するarrayファイルを決定
            long match_one;
            if( type == SearchType.full || type == SearchType.forward )
            {
                match_one = dict_instance_w.bsearch_match_one(type, search_word);
            }else{
                // こちらはtrans系では使わないが、将来に備えいれておく
                match_one = dict_instance_c.bsearch_match_one(type, search_word);
            }

            if(KJ_dict.HashAvailable){
                // Hashを使用する場合、今検索した結果を記憶する。
                if(type == SearchType.forward){
                    if(match_one == -1){
                        // 存在しない
                        KJ_dict.dictCheckForwardHash[search_word] = false;
                    }else{
                        KJ_dict.dictCheckForwardHash[search_word] = true;
                    }
                }else  if(type == SearchType.full){ 
                    if(match_one == -1){
                        // 存在しない
                        KJ_dict.dictCheckFullHash[search_word] = false;
                    }else{
                        KJ_dict.dictCheckFullHash[search_word] = true;
                    }
                }else{
                    Debug.Assert(false) ;
                }
            }
            
            if(match_one == -1){
                return false ;
            }else{
                return true ;
            }
        }
        
        //--------------------------------------------------------------------
        // その他正規表現検索
        static public SearchResult RegexSearch(SearchType type , string searchword)
        {
            SearchResult result;
            
            
            char[] any1 = new char[]{'.', '*', '?', '+', '[', '{' } ; 
            char[] any2 = new char[]{'.', '*', '?', '+', ']', '}' } ; 
            
            int leftPos  = searchword.IndexOfAny(any1) ;
            int rightPos = searchword.LastIndexOfAny (any2) ;
            int strLen   = searchword.Length ;
                                      
            // あいう[か-く]まみ
            // 0 1 2 34 56 78 9   
            //  leftPos:3    rightPos:7  strLen:9   
            //    head : あいう   
            //    tail : まみ
          //  if(leftPos+1==rightPos){
          //      return null;
          //  }

            string head = searchword.Substring(0, leftPos);
            string tail = searchword.Remove(0, rightPos + 1);

            KJ_dict.filterRegexword = searchword;
            
            // filterのデリゲート生成                       
            FilterDelegate dictFilter = new FilterDelegate(KJ_dict.RegexFilter );
            KJ_dict.SetFilter(dictFilter);

            // 長い方を優先に、前方一致または後方一致をかけ、
            // その結果からfilterを使い絞込みを行う。
            if(head.Length >= tail.Length){
                result = KJ_dict.DictSearch( SearchType.forward , head );
            }else{
                result = KJ_dict.DictSearch( SearchType.backward , tail );
            }
            
            
            return result;
        }
        
        //-------------------------------------------------------------------------
        // "*"正規表現用のフィルタ  (前方一致、後方一致を使う簡易型)
  //      static private bool asterRegexFilter(DocumentData doc){  
  //
  //          string [] checkTags = { "key1", "word1", "pronun1", 
  //                                  "key2", "word2", "pronun2"    }; 
  //                                  
  //          foreach ( String tag in checkTags ) {
  //              string key = doc.GetData(tag);
  //              if(key.StartsWith(KJ_dict.filterHeadword) && 
  //                 key.EndsWith(KJ_dict.filterTailword)       ){
  //                  return true;
  //              }
  //          }
  //          return false;
  //      }
        //--------------------------------------------------------------------
        // 正規表現用のフィルタ   (いろいろできるけど遅い)
        static private bool RegexFilter(DocumentData doc){  

            string [] checkTags = { "key1", "word1", "pronun1", 
                                    "key2", "word2", "pronun2"    }; 

            Regex regex = new Regex("^" + KJ_dict.filterRegexword + "$");
                        
            foreach ( String tag in checkTags ) {
                string key = doc.GetData(tag);
                if(regex.IsMatch(key)){
                    return true;
                }
            }
            return false;
        }
        //--------------------------------------------------------------------
        

        
    } // End class KJ_dict
    
    
} // End namespace HYAM.KJ_dict

