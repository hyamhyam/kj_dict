// Copyright 2004, 2009 hyam <hyamhyam@gmail.com>
// and it is under GPL
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2, or (at your option)
// any later version.
using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.InteropServices; 

#if !EDICT
using HYAM.Lingua;
#endif

namespace HYAM.KJ_dict
{
    //------------------------------------------------------------------------
    // 設定を保存するためのclass
    public class KJ_form_Setting 
    {
        public bool withPronunciation;       // 発音を表示するか？
        public int  PronunciationType;       // 発音の表示形式（1:kana 2:alpha）
        public int  TargetLang;              // 検索対象（1:all 2:kr 3:jp）
        public string  CultureName;          // ラベルの表示言語
                                             //  （"en-US" or "ko-KR" or "ja-JP" ）
        public bool debugInfo;               // デバッグ情報を表示するか？
        public bool except9999;              // cost:9999を除く
        public bool except8888;              // cost:8888を除く
        public bool ClipboardView;           // Clipboard監視
        public bool CompactForm;             // コンパクトモード
        public System.Drawing.Size  FormSize;           // FormSize
        public System.Drawing.Size  CompactFormSize;    // CompactFormSize
        
        public string      font;
   //     public string      foreColor1;
        
        
        // 設定保存先のファイル名
        public string SettingFileName
#if !EDICT
           = "KJ_dictform_setting.xml"; 
#else
           = "KJ_edict_setting.xml"; 
#endif        
                
        // 設定を保存
        public void Save()
        {

            //書き込むオブジェクトの型を指定する
            System.Xml.Serialization.XmlSerializer serializer1 =
                new System.Xml.Serialization.XmlSerializer(typeof(KJ_form_Setting));

            try{               
                //ファイルを開く
                System.IO.FileStream fs1 =
                    new System.IO.FileStream(SettingFileName, 
                                             System.IO.FileMode.Create);
            
                //シリアル化し、XMLファイルに保存する
                serializer1.Serialize(fs1, this);

                //閉じる
                fs1.Close();
            }catch{
                // read only media上でも動作させるためNOP
            }
        }

        
    } // end of class KJ_form_Setting
    

}
