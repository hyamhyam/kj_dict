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
    

    // SuffixArrayを使いデータを検索するclass
    public class KJ_SuffixArray
    {
        // 辞書ファイル用  (通常 KJ_dict.yml)
//        static public FileStream   dict_fs ;
//        static public StreamReader dict_reader;
                     // StreamReaderにSeekは効かないので使いまわし不可
          static  byte[] ByteArray=new byte[256];
          static  UTF8Encoding utf8 = new UTF8Encoding();

                     
        //===========================================================================
        //
        //  public methods
        //

        //---------------------------------------------------------------------
        // suffix arrayのindex番目(先頭0)に該当する辞書ファイルの文字列を返す。
        //
        //  KJ_dict.yml.c.ary  etc
        //   +-------------+
        //   | 0B 00 00 00 |  0
        //   +-------------+                          KJ_dict.yml
        //   | 19 00 00 00 |  1 ----------+            +-----------------  -+    
        //   +-------------+     index=1  |            |key1: 무선랜        |
        //   | 4A 00 00 00 |  2           |            +-----------------   |    
        //   +-------------+              |            |        :           |   
        //   |      :      |              |            +-----------------   |    
        //                                |            |key1: 병렬처리      | 0x19
        //                                |            +-----------------   |    
        //                                +----------> |key2: 並列処理     -+
        //                                             |        |   
        //                                             |       dict_pointer      
        //
        //
        static public String GetWord(FileStream dict_fs , BinaryReader array_reader, long index)
        {
            long dict_pointer = GetSeekPointer(array_reader , index) ;
            String word = GetWordbyPtr(dict_fs , dict_pointer);
            return(word);
        }

        
        //---------------------------------------------------------------------
        // suffix arrayのindex番目(先頭0)の値を返す。
        //
        //  KJ_dict.yml.c.ary  etc
        //   +-------------+
        //   | 0B 00 00 00 |  0
        //   +-------------+           GetSeekPointer(reader, 1) ---> 25 (0x19)
        //   | 19 00 00 00 |  1
        //   +-------------+
        //   | 4A 00 00 00 |  2
        //   +-------------+
        //   |      :      |
        //
        static public long GetSeekPointer(BinaryReader array_reader, long index)
        {
            long seek_pos = index * 4;
            array_reader.BaseStream.Seek(seek_pos, SeekOrigin.Begin); 
            long dict_pointer = array_reader.ReadInt32();   

            return(dict_pointer);
        }

        
        //===========================================================================
        //
        //  private methods
        //
        
    
        //---------------------------------------------------------------------
        // 辞書ファイルのdict_pointer位置から改行までの語を取り出す。
        //
        //  KJ_dict.yml
        //   +------------------       
        //   |key1: 무선랜
        //   +------------------       
        //   |        :               
        //   +------------------       
        //   |key1: 병렬처리
        //   +------------------       
        //   |key2: 並列処理
        //   |        |   
        //   |       dict_pointer           return value is "列処理"
        //               
        //       
        //      *注：このmethodは性能が悪い。
        //           性能向上を考えるならここを見直す(2005.10)
        //           （発行回数が多いだけでしかたないかも）
        //
        static private String GetWordbyPtr(FileStream dict_fs , long dict_pointer)
        {
//            StreamReader dict_reader = new StreamReader(dict_fs, Encoding.UTF8 );
//            dict_reader.BaseStream.Seek(dict_pointer, SeekOrigin.Begin); 
//            String word = dict_reader.ReadLine();   
//            return(word);


            // 毎回newしない方式を試す
            
            dict_fs.Seek(dict_pointer, SeekOrigin.Begin);

            // 検索語は256バイト以下という制限がつくが...
            dict_fs.Read(ByteArray, 0, 256);
            String decodedString = utf8.GetString(ByteArray);

            string word = decodedString.Substring(0, decodedString.IndexOf("\n") - 1 ) ;
          

            return(word);

        }



    } // End class KJ_SuffixArray
    
    
} // End namespace HYAM.KJ_dict

