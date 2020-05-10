// Copyright 2004,2008 hyam <hhhyam@ybb.ne.jp>
// and it is under GPL
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2, or (at your option)
// any later version.

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Reflection;

[assembly: AssemblyTitle("make suffix array for KJ_dict")]

namespace HYAM.KJ_dict {
    
//----------------------------------------------------------    
//  KJ_dict.yml から検索用のsuffix arrayを作成する
//
//   USAGE:  make_array.exe KJ_dict.yml
//
class MakeArray{

    #if DEBUG_ARRAYLOG
    // logファイル用
    static private FileStream   log_fs ;
    static private StreamWriter log_w;
    #endif
    
    static  void Main( string[] args ) {  
        bool CheckCost = false;
        
        if(args.Length != 1 && args.Length != 2 && args.Length != 3 && args.Length != 4){
            Console.WriteLine("USAGE: make_array.exe yml_file");
            Console.WriteLine("       make_array.exe yml_file index_tags");
            Console.WriteLine("       make_array.exe yml_file index_tags index_file_name");
            Console.WriteLine("       make_array.exe yml_file index_tags index_file_name \"CheckCost\"");
            Console.WriteLine("         tagsは:で区切ります。(タグは前方一致で判定)");
            return;
        }
        
        if( !args[0].EndsWith(".yml")){
            Console.WriteLine("first argument must be yml_file");
            return;
        }
        
        if(args.Length == 4  ){
            if(args[3] != "CheckCost"){
                Console.WriteLine("argument4 must be \"CheckCost\" yet.");
                return;
            }
            CheckCost = true ;
        }

        // 入力yml file
        String ymlfile = args[0];
 
        #if DEBUG_ARRAYLOG
        log_fs  = new FileStream("Log_array.txt", 
                                 FileMode.Create, 
                                 FileAccess.Write);
        log_w   = new StreamWriter(log_fs, Encoding.UTF8);
        log_w.Write("MakeArray Main Start" + "\n" );
        #endif

        // indexをつけるタグ
        string [] tags = {"key", "word", "pronun"};  // default
        
        if(args.Length >= 2){
            // 引数でタグ指定あり
            tags = args[1].Split(':'); // index化するタグを引数で指定されたものに変更
        }
                
        // 出力index array file
        String w_array_file = "";
        String c_array_file = "";
        if(args.Length >= 3){
            w_array_file = args[2] + ".w.ary";
            c_array_file = args[2] + ".c.ary";
        }else{
            w_array_file = ymlfile + ".w.ary";
            c_array_file = ymlfile + ".c.ary";
        }

        if (!System.IO.File.Exists(ymlfile) == true)
        {
            Console.WriteLine(ymlfile + " is not exist.");
            return;
        }

        Console.WriteLine("===== ");
        Console.WriteLine( ymlfile + " -->  " + w_array_file + " "
                                              + c_array_file 
                                              );
        
        FileStream yml_fs = new FileStream(ymlfile, FileMode.Open, FileAccess.Read);
        StreamReader yml_reader = new StreamReader(yml_fs, Encoding.UTF8 ); 
        
        FileStream    wary_fs = new FileStream(w_array_file, FileMode.Create, FileAccess.Write);
        BinaryWriter  w_writer  = new BinaryWriter(wary_fs);
        
        FileStream    cary_fs = new FileStream(c_array_file, FileMode.Create, FileAccess.Write);
        BinaryWriter  c_writer  = new BinaryWriter(cary_fs);


        ArrayList w_array = new ArrayList();
        ArrayList c_array = new ArrayList();

        String str;
        
        int word_cnt=0, char_cnt=0;
        
      //  int offset=0;   //  KJ_dict.ymlのBOMの分 3バイトを予め設定しない 2005.11.04
        int offset=3;   //  KJ_dict.ymlのBOMの分 3バイトを予め設定 2008.07.05
        
        
        // "--"区切りのグループ単位のスタック
        ArrayList w_array_group = new ArrayList();
        ArrayList c_array_group = new ArrayList();
        
        // costチェック用
        //  （CheckCost時、cost1:9999 && cost2:9999であるグループはインデクス化しない)
        int cost1=0;
        int cost2=0;

        int record_cnt=0;
        // 1行毎に処理
        while( ( str = yml_reader.ReadLine())  != null  ){

            record_cnt++;
            
            int indexpoint;
            int reclength = ByteLength(str) + 2 ;  // +2は \r\n
            
            if(str.StartsWith("---")){
                // 全体のリストに追加する
                if(CheckCost && cost1==9999 && cost2==9999) {
                    // nop
                    // この場合はindex化しない
                }else{
                    foreach ( WordList ia in w_array_group ){
                        w_array.Add(ia);
                    }        
                    foreach ( WordList ia in c_array_group ){
                        c_array.Add(ia);
                    }     
                }   
                
                // 再初期化
                w_array_group = new ArrayList();
                c_array_group = new ArrayList();
                cost1=0;
                cost2=0;

            }

            // Costをチェックする場合
            if(CheckCost && str.StartsWith("cost1: 9999")){
                cost1=9999;
            }
            if(CheckCost && str.StartsWith("cost2: 9999")){
                cost2=9999;
            }

            if( IsIndexedTag(str, tags) ) {  // index化すべきタグか？
                if(reclength > 256){
                    Console.WriteLine("over 256 bytes record cannot be used.");
                    Console.WriteLine("stop!" );
                    Console.WriteLine("record_cnt=" + record_cnt);
                    return ;
                }
                
                int tag_end = str.IndexOf(":") + 1 ;     // key1: などの:までの長さ

                tag_end = tag_end + 1;     // 2005.09.20 
                                           //   "key1: xxxxx"
                                           //   のように空白１つ:の後に追加
                
                indexpoint = offset + tag_end ;

                str = str.Remove(0, tag_end); // タグ削除
                w_array_group.Add( new WordList(indexpoint, str) );
                c_array_group.Add( new WordList(indexpoint, str) );
//                w_array.Add( new WordList(indexpoint, str) );
//                c_array.Add( new WordList(indexpoint, str) );
                
                
                word_cnt++;
                char_cnt++;
                
                // 文字単位にc_index_arrayに追加
                while(true){
                    if(str.Length==0){
                        break;   // strがなくなったら抜ける
                    }
                    String substr = str.Substring(0, 1);  // 先頭1文字取り出し
                    str = str.Remove(0, 1);               // 先頭1文字削除
                    int len = ByteLength(substr);
                    indexpoint += len;
                    c_array_group.Add( new WordList(indexpoint, str) );
//                    c_array.Add( new WordList(indexpoint, str) );
                    char_cnt++;
                }
                
            } // end of if  IsIndexedTag

            offset += reclength;

        } // end of while
        

        // ソート
        w_array.Sort(new WordComparer());
        c_array.Sort(new WordComparer());
        
        
        // 出力  (語単位のarray)
        foreach ( WordList ia in w_array ){
            w_writer.Write(ia.indexpoint);
            #if DEBUG_ARRAYLOG
            log_w.Write(ia.word + ":" + ia.indexpoint + "\n" );
            #endif
        }        
        // 出力  (文字単位のarray)
        foreach ( WordList ia in c_array ){
            c_writer.Write(ia.indexpoint);
        }        
        
        
        // 終了処理
        yml_reader.Close();
        w_writer.Close();       
        c_writer.Close();       
        
        Console.WriteLine("word counter = " + word_cnt );
        Console.WriteLine("char counter = " + char_cnt );
                                             
    }  // end of main

    //--------------------------------------------------------
    static int ByteLength(String str){
        byte [] bytesData = System.Text.Encoding.GetEncoding(65001).GetBytes(str);
        return(bytesData.Length);
    }
    //--------------------------------------------------------
    // index化すべきタグならtrue (前方一致ならtrue)
    //    tags = {"key", "word", "pronun"};  なら key1 key2などtrue
    static bool IsIndexedTag(String str, string [] tags){
        foreach ( String tag in tags ) {
            if(str.StartsWith(tag)){
                return true;
            }
        }
        return false ;
    }
    
}  // end of class MakeArray


// ソート用 コンペア ---------------------------------------------------
class WordComparer : IComparer {
    public int Compare(object l, object r) {
        WordList left  = l as WordList;
        WordList right = r as WordList;
        
        // 2005.08.19 空白を削除してから比較する
        string  left_nospace = left.word.Replace(" ", "");
        string  right_nospace = right.word.Replace(" ", "");
        
        if(left_nospace == right_nospace){             // 2005.08.19
//        if(left.word == right.word){                 // 2005.08.19
            return(0);
        }
        
        // 単なるCompareでは日本語・ハングルの比較で場合によって逆転するので（？）
        // CompareOrdinalでないと不可
        return(String.CompareOrdinal(left_nospace, right_nospace));    // 2005.08.19
//        return(String.CompareOrdinal(left.word, right.word));        // 2005.08.19

//      return(left.CompareTo(right));    (*1)
    }
    
}  // end of class WordComparer

class WordList : IComparable {
    public int indexpoint;
    public String word;
    
    public WordList(int indexpoint, String word){
        this.indexpoint = indexpoint;
        this.word = word;
    }
    
    // 単純なString比較でいいので，直接使わない。
    // 比較処理が1行で終わらず，(*1)のように書く必要がある場合はちゃんと書く。
    public int CompareTo(object obj){
        return(0); // dummy
        //WordList o = obj as WordList ;
        //return(String.CompareOrdinal(this.word, o.word));
    }
}


} // end of namespace
