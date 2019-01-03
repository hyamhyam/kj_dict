// Copyright 2004,2009 hyam <hhhyam@ybb.ne.jp>
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

namespace HYAM.Lingua 
{

    //---------------------------------------------------------------------
    // てきとうな，文字や文字列の種別判定 (文字列なら先頭1文字で判断)
    public class CodeCheck{
    
        //-----------------------------
        static public bool IsDigit(String str){
            if(str.Length == 0) {
                return(false);  
            }
            char  ch =  str[0];
            bool rtn = IsDigit( ch ) ;
            return( rtn );
        }
        static public bool IsDigit(char ch) {
            if(Char.IsDigit(ch)){
                return(true);  
            }else{
                return(false);  
            }
        }
        
        //-----------------------------
        static public bool IsAlpah(String str){
            if(str.Length == 0) {
                return(false);  
            }
            char  ch =  str[0];
            bool rtn = IsAlpah( ch ) ;
            return( rtn );
        }
        static public bool IsAlpah(char ch) {
            if((ch >= '\u0041'  &&           // 'A'
                ch <= '\u005A'     ) ||      // 'Z'
               (ch >= '\u0061'  &&           // 'a'
                ch <= '\u007A'     )   ){    // 'z'
                return(true);  
            }
            else
            {
                return(false);  
            }
        }
        //-----------------------------
        static public bool IsHangul(String str){
            if(str.Length == 0) {
                return(false);  
            }
            char  ch =  str[0];
            bool rtn = IsHangul( ch ) ;
            return( rtn );
        }
        static public bool IsHangul(char ch) {
            // Hangul Syllables
            if( ch >= '\uAC00'  &&     // U+AC00 Hangul Kiyeok+A
                ch <= '\uD7A3'     ){  // U+D7A3 Hangul Hieuh+I+Hieuh
                return(true);  
            }
            else
            {   // Hangul Jamo
                if((ch >= '\u1100'  && ch <='\u1112') ||
                   (ch >= '\u1161'  && ch <='\u1175') ||
                   (ch >= '\u11A8'  && ch <='\u11C2')   )    {
                    return(true);  
                }else{
                    return(false);  
                }
            }
        }
        //-----------------------------
        static public bool IsKanji(String str){
            if(str.Length == 0) {
                return(false);  
            }
            char  ch =  str[0];
            bool rtn = IsKanji( ch ) ;
            return( rtn );
        }
        static public bool IsKanji(char ch) {
            // CJK Unified Ideographs
            //   (4E00..9FFF;より広めに取っている)
            if( ch >= '\u4E00'  &&    
                ch <= '\uFA2D'  && 
                !IsHangul(ch)      ){   // ハングル除く
                return(true);  
            }
            else
            {
                return(false);  
            }
        }

        //-----------------------------
        static public bool IsPunctuation(String str){
            if(str.Length == 0) {
                return(false);  
            }
            char  ch =  str[0];
            bool rtn = IsPunctuation( ch ) ;
            return( rtn );
        }
        static public bool IsPunctuation(char ch) {
            if( ch == '、'  ||
                ch == '，'  ||    
                ch == '。'  ||    
                ch == ','   ||    
                ch == '.'         ){  
                return(true);  
            }
            else
            {
                return(false);  
            }
        }
        //-----------------------------
        static public bool IsKatakana(String str){
            if(str.Length == 0) {
                return(false);  
            }
            char  ch =  str[0];
            bool rtn = IsKatakana( ch ) ;
            return( rtn );
        }
        static public bool IsKatakana(char ch) {
            if( (ch >= '\u30A1'  &&      //  Katakana small a
                 ch <= '\u30FA'  ) ||    //  Katakana Vo
                 ch == '\u30FC'    ||    //  Prolonged Sound Mark
                ( ch >= '\uFF66'  &&     //  Halfwidth Katakana wo
                 ch <= '\uFF9F'  ) ){    //  Halfwidth Katakana semi-voiced 
                return(true);  
            }
            else
            {
                return(false);  
            }
        }
        //-----------------------------
        static public bool IsHiragana(String str){
            if(str.Length == 0) {
                return(false);  
            }
            char  ch =  str[0];
            bool rtn = IsHiragana( ch ) ;
            return( rtn );
        }
        static public bool IsHiragana(char ch) {
            if( ch >= '\u3041'  &&    
                ch <= '\u3094'     ){  
                return(true);  
            }
            else
            {
                return(false);  
            }
        }
        
        static public bool IsAllHiragana(string str) {
            return Regex.IsMatch(str, @"^\p{IsHiragana}*$");
        }
        
        static public bool IsAllKatakana(string str) {
            return Regex.IsMatch(str, @"^\p{IsKatakana}*$");
        }
        
        static public bool IsJapanese(String str){
            return(false);  // 今は常にfalse
        }
   
    }  // End class CodeCheck

    // see also 
    //   http://www.unicode.org/Public/UNIDATA/Blocks.txt
    //   such as 
    //     static bool IsHiragana(string str) {
    //         return Regex.IsMatch(str, @"^\p{IsHiragana}");
    //     }
    //     static bool IsKanji(string str) {
    //         return Regex.IsMatch(str, @"^\p{IsCJKUnifiedIdeographs}");
    //         // return Regex.IsMatch(str, @"^\p{IsCJKUnifiedIdeographs}*$");
    //         // 末尾まで全てチェックするなら
    //     }
    //


} // End namespace KJ_util

