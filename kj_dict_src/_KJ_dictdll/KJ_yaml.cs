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
    // YAML(風の)テキストを操作するためのクラス
    //
    //      YAMLであるとは 謳わない...
    //
    public class KJ_yaml
    {
        //static private FileStream dict_fs ;
        //static private StreamReader dict_reader;
        

        //---------------------------------------------------------------------
        // 辞書ファイルのdict_pointer位置が入っているブロック(---で囲まれた範囲)をまとめて返す。
        static public DocumentData GetBlock(FileStream dict_fs, long dict_pointer)
        {

            // 前の"---"行まで逆に戻る
            dict_pointer = BacktoBlockDelimiter(dict_fs, dict_pointer);
            
            // "---"の先頭までSeek
            StreamReader dict_reader = new StreamReader(dict_fs, Encoding.UTF8 );
            dict_fs.Seek(dict_pointer , SeekOrigin.Begin);
            
            String line = dict_reader.ReadLine();  // 前の"---"行は捨てる
        
            // 次の---が出てくるまで出力。（---で囲まれた範囲を返す）
            DocumentData blockdata = new DocumentData();
            blockdata.dict_pointer = dict_pointer  ;
            while(true)
            {
                line = dict_reader.ReadLine() ;  
                if(line.StartsWith("---"))
                {
                    break;     // 次の "---"が出たら終了
                }
                int colonPos = line.IndexOf(":");
                if(colonPos < 0){
                    continue;
                }
                // :の前後をkeynameとvalueに格納
                string keyname = line.Substring(0, colonPos);
                string value   = line.Remove(0, colonPos + 2); // 2005.09.20 (:の後ろに空白追加)
                  //   string value   = line.Remove(0, colonPos + 1);
                blockdata.SetData(keyname, value) ;
            }
        
            return(blockdata);
        }

        //---------------------------------------------------------------------
        // dict_pointer位置からその前にある"---"行まで戻る
        static public long BacktoBlockDelimiter(FileStream dict_fs, long dict_pointer)
        {
            int back=1;
            int p1=0, p2=0, p3=0,  c=0;
        
            while(true)
            {
                // 1バイトずつ戻る
                dict_fs.Seek((dict_pointer - back ), SeekOrigin.Begin);
                
                p3 = p2;
                p2 = p1;
                p1 = c;
                
                c = dict_fs.ReadByte();
                // c = dict_reader.Read() ; // 1文字read  
                if(c==10 && p1==45 && p2==45 && p3==45)
                {   // "---"まで戻ったら抜ける
                    break;
                }
                if( (dict_pointer - back) == 0 ) 
                {     // ファイルの先頭まで戻ったら抜ける
                    break;
                }
            
                back++;
            }
            
            // ptrはc=10の位置（改行コードの位置）。
            // "---"の先頭"-"にするため+1する。
            dict_pointer = dict_pointer - back + 1; // Blockの先頭位置を保存するため返す
            
         
            return( dict_pointer );
        }


    } // End class KJ_dict

} // End namespace HYAM.KJ_dict
