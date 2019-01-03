// Copyright 2004, 2008 hyam <hhhyam@ybb.ne.jp>
// and it is under GPL
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2, or (at your option)
// any later version.

using System;
using System.Collections;
using System.Diagnostics ;

namespace HYAM.Lingua {
//-----------------------------------------------------------------------------

    

public class Hangul {
    //------------------------------------------------------------------------
    // 
    public static string [] ConsonantList = { 
     //  1     2      3     4     5     6     7     8    9    10
        "G",  "GG",  "N",  "D",  "DD", "R",  "M",  "B", "BB", "S",
        "SS",  "",   "J", "JJ",  "C",  "K",  "T",  "P", "H",
    };
    public static string [] ConsonantPartsList = { 
     //  1     2      3     4     5     6     7     8    9    10
        "x1", "x2", "x3", "x4", "x5", "x6", "x7", "x8", "x9", "xa", 
        "xb", "xc", "xd", "xe", "xf", "xg", "xh", "xi", "xj",
    };
    public static string [] ConsonantKeyList = { 
     //  1     2      3     4     5     6     7     8    9    10
        "r",  "R",   "s",  "e",  "E",  "f",  "a",  "q", "Q", "t",
        "T",  "d",   "w",  "W",  "c",  "z",  "x",  "v", "g",
    };
    public static string [] ConsonantJamoList = { 
//0x3131 -- Hangul Compatibility Jamo 
        "ㄱ",  "ㄲ",  "ㄴ",  "ㄷ",  "ㄸ",  "ㄹ",  "ㅁ",  "ㅂ", "ㅃ", "ㅅ",
        "ㅆ",  "ㅇ",  "ㅈ",  "ㅉ",  "ㅊ",  "ㅋ",  "ㅌ",  "ㅍ", "ㅎ"
    };

    public static string [] VowelList  = { 
     //  1       2     3     4     5      6     7      8     9     10
        "A",   "AE", "YA", "YAE", "EO",  "E",  "YEO", "YE", "O",  "WA",
        "WAE", "OE", "YO", "U",   "WEO", "WE", "WI",  "YU", "EU", "YI",
        "I"
    };
    public static string [] VowelPartsList  = { 
     //  1       2     3     4     5      6     7      8     9     10
        "y1",  "y2", "y3", "y4",  "y5",  "y6", "y7",  "y8", "y9", "ya",  
        "yb",  "yc", "yd", "ye",  "yf", "yg",  "yh",  "yi", "yj", "yk",
        "yl"
    };
    public static string [] VowelKeyList  = { 
     //  1       2     3     4     5      6     7      8     9     10
        "k",   "o",   "i",  "O",  "j",   "p",  "u",   "P",  "h",  "hk",
        "ho",  "hl",  "y",  "n",  "nj",  "np", "nl",  "b",  "m",  "ml",
        "l"
    };
    public static string [] VowelJamoList  = { 
// 314F--
     //  1       2     3     4     5      6     7      8     9     10
       "ㅏ",  "ㅐ",  "ㅑ",  "ㅒ", "ㅓ",  "ㅔ", "ㅕ",  "ㅖ", "ㅗ" , "ㅘ",
       "ㅙ",  "ㅚ",  "ㅛ",  "ㅜ", "ㅝ",  "ㅞ", "ㅟ",  "ㅠ", "ㅡ" , "ㅢ",
      "ㅣ" 
    };
    
    public static string []  PachimList = { 
     //  1     2      3     4     5     6     7     8    9    10
        "",   "G",   "GG", "GS", "N",  "NJ", "NH", "D", "L", "LG", 
        "LM", "LB",  "LS", "LT", "LP", "LH", "M",  "B", "BS", "S",
        "SS",  "NG", "J",  "C",  "K",  "T",  "P",  "H"
    };
    public static string []  PachimPartsList = { 
     //  1     2      3     4     5     6     7     8    9    10
        "",   "z1", "z2", "z3", "z4", "z5", "z6", "z7", "z8", "z9", 
        "za", "zb", "zc", "zd", "ze", "zf", "zg", "zh", "zi", "zj", 
        "zk", "zl", "zm", "zn", "zo", "zp", "zq", "zr",
    };
    public static string []  PachimKeyList = { 
        "",    "r",   "R",  "rt", "s",  "sw",  "sg",  "e", "f",  "fr", 
        "fa",  "fq",  "ft", "fx", "fv", "fg",  "a",   "q", "qt", "t",
        "T",   "d",   "w",  "c",  "z",  "x",   "v",   "g"
    };
    public static string []  PachimJamoList = { 
        // 0x3131 --
        "",   "ㄱ",  "ㄲ", "ㄳ", "ㄴ",  "ㄵ", "ㄶ",  "ㄷ", "ㄹ", "ㄺ", 
        "ㄻ", "ㄼ", "ㄽ", "ㄾ", "ㄿ", "ㅀ",  "ㅁ",  "ㅂ", "ㅄ", "ㅅ",
        "ㅆ",  "ㅇ", "ㅈ", "ㅊ", "ㅋ",  "ㅌ", "ㅍ",  "ㅎ"
    };
    //------------------------------------------------------------------------
    public enum Pachim {
        Yes,
        No,       
        Both      
    } ;
    

    //------------------------------------------------------------------------
    // instance 
    public string    text;      // base text
    public string [] kRomanArray;    // romanaized char by array
    public string    katakana;     
    
    //------------------------------------------------------------------------
    // constructor
    public Hangul(string text ) {
        this.text   = text;
        this.kRomanArray   = Hangul.Hangul2kRomanArray(this.text);
        this.katakana      = Hangul.kRomanArray2Kana(this.kRomanArray);
    }

    


    //------------------------------------------------------------------------
    //  static public method 
    //------------------------------------------------------------------------
    // Hangul ---> Japanese Katakana   (ex. 아이돌 --->  アイドル )
    static public string  Hangul2Kana( string  hangul ) {
        string [] kRomanArray   = Hangul.Hangul2kRomanArray(hangul);
        string    katakana      = Hangul.kRomanArray2Kana(kRomanArray);
        
        return katakana ;
    }
    //------------------------------------------------------------------------
    static public bool withPachim( char ch ) {
        int code = ch;
        code = code - 0xAC00;
        if(code%28 == 0){
            return(false);
        }else{
            return(true);
        }
    }
    static public bool withPachim( string str ) {
        if(str == ""){
            return false;
        }
        return withPachim(str[str.Length - 1]);
    }
    //------------------------------------------------------------------------


    //------------------------------------------------------------------------
    // ハングル１文字をAlpha化
    // 돌 ---> DOL 
    static public string  HangulChar2kRoman( char ch ) {
        if(!CodeCheck.IsHangul(ch)) {
            // Not Hangul  そのまま返す 
            return char.ToString(ch); 
        }
        
        int code = ch;
        
        if(code >= 0x1100  && code <=0x1112 ) {
            code = code - 0x1100;
            return (ConsonantList[code]);
        }
        if(code >= 0x1161  && code <=0x1175  ) {
            code = code - 0x1161;
            return (VowelList[code]);
        }
        if(code >= 0x11A8  && code <=0x11C2  ) {
            code = code - 0x11A8;
            return (PachimList[code]);
        }
        
        code = code - 0xAC00;
        
        int c1 =  code/(21*28) ;
        int c2 =  (code/28) % 21 ;
        int c3 =  code%28 ;
        
        return (ConsonantList[c1]+VowelList[c2]+PachimList[c3]) ;
    }
    //------------------------------------------------------------------------
    // ハングル１文字をキー化(2ボル式)
    // 돌 ---> DOL 
    static public string  HangulChar2Key( char ch ) {
        if(!CodeCheck.IsHangul(ch)) {
            // Not Hangul  そのまま返す 
            return char.ToString(ch); 
        }
        
        int code = ch;
        
        if(code >= 0x1100  && code <=0x1112 ) {
            code = code - 0x1100;
            return (ConsonantKeyList[code]);
        }
        if(code >= 0x1161  && code <=0x1175  ) {
            code = code - 0x1161;
            return (VowelKeyList[code]);
        }
        if(code >= 0x11A8  && code <=0x11C2  ) {
            code = code - 0x11A8;
            return (PachimKeyList[code]);
        }
        
        code = code - 0xAC00;
        
        int c1 =  code/(21*28) ;
        int c2 =  (code/28) % 21 ;
        int c3 =  code%28 ;
        
        return (ConsonantKeyList[c1] + VowelKeyList[c2] + PachimKeyList[c3]) ;
    }

    //------------------------------------------------------------------------
    // ローマ字形式(1文字分)をハングルの文字に。
    // DOL ---> 돌 
    static public string  kRoman2Hangul( string kRoman ) {
        int [] keyindex = kRoman2Hangulnumber(kRoman);
        return ( Hangulnumber2Hangul(keyindex) ) ;
    }
    
    // 字母のindexの番号をハングルにする。
    //   (3, 8, 9) ---> 돌
    static private string  Hangulnumber2Hangul( int [] keyindex ) {
        // 不完全な字の場合は、個々の字母で返す
        if(keyindex[0] == -1 || keyindex[1] == -1 || keyindex[2] == -1 ){
            string jabo="";
            if(keyindex[0] != -1){
                jabo += ConsonantJamoList[keyindex[0]];
            }
            if(keyindex[1] != -1){
                jabo += VowelJamoList[keyindex[1]];
            }
            if(keyindex[2] != -1){
                jabo += PachimJamoList[keyindex[2]];
            }
            return ( jabo ) ;
        }
        
        // ハングルの組み立て -----------------
        int hangulCode = 0xAC00 + ( keyindex[0]*21*28 )
                                + ( keyindex[1]*28 ) + keyindex[2] ;
                                
        char hangulChar = Convert.ToChar(hangulCode) ;
        string hangul = char.ToString(hangulChar);
        
        return hangul ;
    }
    //------------------------------------------------------------------------
    // ローマ字形式(1文字分)を字母のindexの番号にする。
    //   DOL ---> D + O + L ---> (3, 8, 9)
    //   A   ---> (11, 0, 0)
    static public int []  kRoman2Hangulnumber( string kRoman ) {
        int [] keyindex = new int [3]; 
        
        // 子音のチェック -----------------
        int consonatNum=-1;
        if( !isStartsWithVowel(kRoman) ){
            // 母音で始まっていない場合
            int consonantLength=0;
            for(int i=0; i < ConsonantList.Length; i++ ){
                if(ConsonantList[i] == ""){
                    continue;
                }
                if(kRoman.StartsWith(ConsonantList[i])){
                    consonatNum = i;
                    consonantLength = ConsonantList[i].Length;
                    
                    // "G",  "GG"などあるので，無条件breakしてはいけない
                    if(consonantLength==2){
                        break;
                    }
                }   
            }
            kRoman = kRoman.Remove(0, consonantLength);
        }else{
            // Aのように母音で始まる文字
            consonatNum=11; // "" の位置
        }

        // 母音のチェック -----------------
        int vowelNum=-1;
        int vowelLength=0;
        for(int i=0; i < VowelList.Length; i++ ){
            if(kRoman.StartsWith(VowelList[i])){
                vowelNum=i;
                vowelLength=VowelList[i].Length;
                if(vowelLength==3){
                    break;
                }
                if(vowelLength==2){
                    if(VowelList[i] == "YA" || 
                       VowelList[i] == "WA"){
                        // "YAE","WAE"かもしれないので，
                        // breakしては，いけない。
                    }else{
                        break;
                    }
                }
            }
        }
        kRoman = kRoman.Remove(0, vowelLength);
        
        // Pachimのチェック -----------------
        int pachimNum=-1;
        for(int i=0; i < PachimList.Length; i++ ){
            if(kRoman.StartsWith(PachimList[i])){
                pachimNum=i;
                if(PachimList[i].Length==2){
                    break;
                }
            }
        }
        
        // マッチしなかったら -1
        keyindex[0] = consonatNum;
        keyindex[1] = vowelNum;
        keyindex[2] = pachimNum;
        
        return ( keyindex ) ;
    }
    
    //------------------------------------------------------------------------
    // Hangul ---> Korean Roman            (ex.  아이돌 --->  A,I,DOL  )
    //   ハングル１文字は複数キーもまとめる (DOLは１つのstring)
    static public string []  Hangul2kRomanArray( string  hangul ) {
        string [] kRomanArray; 
        kRomanArray  = new string [hangul.Length] ;
        for (int cnt=0; cnt < hangul.Length ;  cnt++ )
        {
            char ch = hangul[cnt];
            string alpha = HangulChar2kRoman(ch);
            kRomanArray[cnt] = alpha;
        }
        return kRomanArray;
    }
    
    //------------------------------------------------------------------------
    // ハングルのstringをkeyのArrayListにする
    //   컴퓨 --> K + EO + M + P + YU
    //  (入力が何文字かわからないのでArrayList)
    static public ArrayList HangulString2KeyArray(string hangul)
    {
        string [] kRomanArray = Hangul2kRomanArray(hangul);
        
        ArrayList keyArray = new ArrayList();
        
        foreach (string roman in kRomanArray){
            int [] keyindex = kRoman2Hangulnumber(roman);
            
            if(keyindex[0] != -1){
                keyArray.Add(ConsonantList[keyindex[0]]);
            }
            if(keyindex[1] != -1){
                keyArray.Add(VowelList[keyindex[1]]);
            }
            if(keyindex[2] != -1 && keyindex[2] != 0 ){  // 0はパッチムなし
                keyArray.Add(PachimList[keyindex[2]]);
            }
        }
        return(keyArray);
    }
    //------------------------------------------------------------------------
    // keyのArrayListをハングルのstringにする。HangulString2KeyArrayの逆。
    //    K + EO + M + P + YU  --> 컴퓨
    //  (入力が何文字かわからないのでArrayList)
    //     HangulString2KeyArray<-->KeyArray2HangulString で 
    //     完全には復元できない場合もある点に注意
    // ★未完成である！！注意！
    static public string KeyArray2HangulString( ArrayList keyArray )
    {
        if(keyArray.Count == 1 && (string)keyArray[0] == "" ){
            return "ㅇ";  //例外
        }
        
        string hangul="";  // 返却値
        
        string consonant = "";  
        string vowel     = "";  
        string pachim    = "";  
        for(int i=0; i<keyArray.Count; ){
            string key=(string)keyArray[i];
            
            if(!isVowel(key) && !isConsonant(key) && !isPachim(key) ){
                break;
            }
            
            if( !isVowel(key) && isConsonant(key) ){
                consonant = key;
                i++;
                if(i==keyArray.Count){ break; }
                key=(string)keyArray[i];
            }
            if( isVowel(key) ){
                vowel = key;
                i++;
                if(i==keyArray.Count){ break; }
                key=(string)keyArray[i];
            }
            if( isPachim(key) ){
                pachim = key;
                i++;
            }

            hangul += kRoman2Hangul(consonant+vowel+pachim) ;
            consonant = ""; 
            vowel     = ""; 
            pachim    = ""; 
        }
        if(consonant != "" || vowel != "" || pachim != "" ){
            hangul += kRoman2Hangul(consonant+vowel+pachim) ;
        }
        
        return hangul;
    }
    //------------------------------------------------------------------------
    // ハングル要素のArrayListをハングルstringにする。[2008/05/03 19:15:52]
    //    x1 + y1 + z4 + x4 + y1 + z4  --> 간단
    //     これはKeyArray2HangulStringと違い完全に復元できる
    static public string PartsArray2HangulString( ArrayList partsArray )
    {
        
        string hangul="";  // 返却値
        
        int consonantPos = -1;  
        int vowelPos     = -1;  
        int pachimPos    = -1;  
        
        for(int i=0; i<partsArray.Count; ){
            string key=(string)partsArray[i];
            
            if( !key.StartsWith("x") && !key.StartsWith("y") && !key.StartsWith("z") ){
                break;
            }
            
            if( key.StartsWith("x") ){
                for(int j=0; j < ConsonantPartsList.Length; j++ ){
                    if(ConsonantPartsList[j] == key){
                        consonantPos = j;
                        break;
                    }
                }
                i++;
                if(i==partsArray.Count){ break; }
                key=(string)partsArray[i]; // 次の文字へ
            }    

            if( key.StartsWith("y") ){
                for(int j=0; j < VowelPartsList.Length; j++ ){
                    if(VowelPartsList[j] == key){
                        vowelPos = j;
                        break;
                    }
                }
                i++;
                if(i==partsArray.Count){ break; }
                key=(string)partsArray[i]; // 次の文字へ
            }

            if((consonantPos != -1 && vowelPos == -1) ||
               (consonantPos == -1 && vowelPos != -1)      ){
                // 子音のみ，母音のみの場合はパッチムは調べない
            }else{
                if( key.StartsWith("z") ){
                    for(int j=0; j < PachimPartsList.Length; j++ ){
                        if(PachimPartsList[j] == key){
                            pachimPos = j;
                            break;
                        }
                    }
                    i++;
                    if(i==partsArray.Count){ break; }
                }
            }
                
            hangul += kParts2Hangul(consonantPos, vowelPos, pachimPos) ;
            
            consonantPos = -1; 
            vowelPos     = -1; 
            pachimPos    = -1; 

        }
        
        hangul += kParts2Hangul(consonantPos, vowelPos, pachimPos) ;
        
        return hangul;
    }
    static private string kParts2Hangul(int consonantPos, int vowelPos, int pachimPos){
        string hangul="";  // 返却値
        char hangulchar;
        int code = 0x0000 ;
        
        if(consonantPos == -1 && vowelPos == -1 && pachimPos == -1){
            code = 0x0000;
        }
        if(consonantPos != -1 && vowelPos == -1 && pachimPos == -1 ){
            code = 0x1100 + consonantPos;
        }
        if(consonantPos == -1 && vowelPos != -1 && pachimPos == -1){
            code = 0x1161 + vowelPos;
        }
        if(consonantPos == -1 && vowelPos == -1 && pachimPos != -1){
            code = 0x11A8 + pachimPos -  1;
        }

        if(consonantPos != -1 && vowelPos != -1 && pachimPos != -1 ){
            code = 0xAC00 + (21 * 28 * consonantPos) + (28 * vowelPos) + pachimPos ;
        }
        if(consonantPos != -1 && vowelPos != -1 && pachimPos == -1 ){
            code = 0xAC00 + (21 * 28 * consonantPos) + (28 * vowelPos);
        }
        
        hangulchar = (char) code;
        hangul = hangulchar.ToString();
        return hangul;
    }
    //------------------------------------------------------------------------
    // Korean Roman Array ---> Japanese Katakana  (ex.   A,I,DOL --->  アイドル )
    // ★未完成である！！注意！
    static private string  kRomanArray2Kana( string [] kRomanArray ) {
        
        string kana="";
        string rest=""; // 前文字残り
        
        for ( int i=0 ; i < kRomanArray.Length ;  i++  ){
            string kRoman = kRomanArray[i];
            
            if(rest!=""){
                kRoman = rest + kRoman;
                rest = "";
            }

            // common converting
            kRoman = ConvertVowel(kRoman);
            kRoman = kRoman.Replace("L",  "R");

            // 先頭の文字
            if(i==0){
                kRoman = Dull2Resonant_on_Head(kRoman);
                kRoman = Geki2Hei_on_Head(kRoman);
            }
            
            // 最後の文字  
            if(i==(kRomanArray.Length - 1) ) {             
                // nop
            }

            //先頭子音の調整
            kRoman = Geki2Hei_on_Head(kRoman);  // 激音の平音化
            if(kana.EndsWith("ッ")){
                // １つ前の最後が促音なら、この語の先頭は清音化
                kRoman = Dull2Resonant_on_Head(kRoman);
            }

            
            // リエゾンの可能性の調査と変換
            if(i!=(kRomanArray.Length - 1) &&          // 最後でなくて，
               isStartsWithVowel(kRomanArray[i+1])){   // 次の語は母音開始
                //末尾の子音があれば外す。
                // (子音をrestに保存し、次の語の頭につなぐ）
                kRoman = RemoveConsonat(kRoman, out rest);
                
            }
            
            
            //終端子音の調整
            kRoman = ConvertLastConsonat(kRoman);
            if(kRoman.EndsWith("NG")){
                if(i==(kRomanArray.Length - 1)){
                    // 最後の文字 NG
                    kRoman=kRoman.Replace("NG", "ング");
                }else{
                    kRoman=kRoman.Replace("NG", "ン");
                }
            }
            
            
            kana = kana + ConvTable.R2K(kRoman);
        } // end of for
        
        
        return kana;
    }
    
    //------------------------------------------------------------------------
    // 2bolsik hangul key ---> Hanngul
    //  ex. fk   ---> 라
    //      fkr  ---> 락
    //      fkrk ---> 라가
    static public string  KeyString2Hangul( string keystring ) {
        string hangul        = "";  // 変換したハングル文字列  
        string oneHangulKey  = "";  // 1文字分のキー
        bool delimit = false;    // 文字の区切りになるか？
        string parts = "";       // ハングルのどの位置を組み立てたか？

        // 一部の大文字を小文字に変換
        keystring = key2lower(keystring);

        for(int i=0; i < keystring.Length; i++) {
            string key = keystring.Substring(i,1);
            string nextkey     = "";      
            string nextnextkey = "";

            // 次の文字，次の次の文字を取れなら取る
            if((i + 1) <= keystring.Length - 1){
                nextkey = keystring.Substring(i+1,1);
                if((i + 2) <= keystring.Length - 1){
                    nextnextkey = keystring.Substring(i+2,1);
                }
            }
            
            //最後のキーなら無条件区切り
            if(i == keystring.Length - 1){
                delimit=true;  // 最後のキー
                parts = "last";
            }

            // チカラ技。あとでどうにかしよう  
            //    : <-- delimiter
            switch(parts)
            {
                case "":
                    // 文字の1キー目
                    if(isConsonantKey(key) && isVowelKey(nextkey)){
                        // nop
                        parts = "Consonant";
                    }else{
                        delimit=true;
                    }
                    break;
                    
                case "Consonant":
                    // keyが母音なのは確定している
                    if(isConsonantKey(key)){ Debug.Assert(false) ; } //論理エラー
                    
                    if(isConsonantKey(nextkey)){
                        if(isVowelKey(nextnextkey)){
                            delimit=true;  // ㄱㅏ(key) : ㄱㅏ (*1)
                        }else{
                            //  ㄱㅏ(key)  ㄱ : ㄱ
                            //  ㄱㅏ(key)  ㄱ   ㅅ :
                            //  ㄱㅏ(key)  ㄱ : ㅅ ㅏ
                            parts = "Vowel";
                        }
                    }
                    if(isVowelKey(nextkey)){
                        if((key=="h" && nextkey=="k")  ||
                           (key=="h" && nextkey=="o")  ||
                           (key=="h" && nextkey=="l")  ||
                           (key=="n" && nextkey=="j")  ||
                           (key=="n" && nextkey=="p")  ||
                           (key=="n" && nextkey=="l")  ||
                           (key=="m" && nextkey=="l")     ) {
                            // 2重母音なので区切らない
                            parts = "Consonant"; // 状態変えない(loopはしない)
                        }else{
                            delimit=true; // ㄱㅏ(key) : ㅏ
                        }
                    }else{
                        // nextkeyは子音
                        if(nextkey=="Q" || nextkey=="W" || nextkey=="E"){
                            // ㅃ ㅉ ㄸはパッチムにはならない
                            delimit=true; 
                        }
                    }
                    break;
                    
                case "Vowel":
                    // keyが子音なのは確定している。
                    // (*1で切ったので)nextkeyも子音なのは確定している。
                    if(isVowelKey(key)){ Debug.Assert(false) ; } 
                    if(isVowelKey(nextkey)){ Debug.Assert(false) ; } 
                    
                    // keyもnextkeyもConsonant
                    if(isVowelKey(nextnextkey)){
                        //  ㄱㅏ  ㄱ(key) : ㄱㅏ
                        //  ㄱㅏ  ㄱ(key) : ㅅㅏ
                        delimit=true;
                    }else{
                        // nextnextkeyは母音でない
                        if((key=="r" && nextkey=="t") ||  // ㄳ
                           (key=="s" && nextkey=="w") ||  // ㄵ
                           (key=="s" && nextkey=="g") ||  // ㄶ
                           (key=="f" && nextkey=="r") ||  // ㄺ
                           (key=="f" && nextkey=="a") ||  // ㄻ
                           (key=="f" && nextkey=="q") ||  // ㄼ
                           (key=="f" && nextkey=="t") ||  // ㄽ
                           (key=="f" && nextkey=="x") ||  // ㄾ
                           (key=="f" && nextkey=="v") ||  // ㄿ
                           (key=="f" && nextkey=="g") ||  // ㅀ
                           (key=="q" && nextkey=="t")     // ㅄ 
                          ){
                           //  ㄱㅏ  ㄱ(key)  ㅅ : ㄱ
                           parts = "Pachim";
                        }else{
                           //  ㄱㅏ  ㄱ(key): ㅊ ㅊ
                            delimit=true;
                        }
                    }
                    break;
                    
                case "Pachim":
                    delimit=true;
                    break;
                    
                case "last":
                    // nop
                    break;
                    
                default:
                    Debug.Assert(false) ; //論理エラー
                    break;

            } // end of switch
            

            if (key != "") {
                oneHangulKey += key;
            }
            
            if( delimit ) {
                hangul += Key2Hangul(oneHangulKey);
                oneHangulKey = "";
                parts = "";
                delimit = false;
            }
        }

        
        return ( hangul ) ;
    }
    
    // 1文字分のキー → ハングル
    static public string  Key2Hangul( string keystring ) {
        int [] keyindex = Key2Hangulnumber(keystring);
        return ( Hangulnumber2Hangul(keyindex) ) ;
    }
    
    // 1文字分のキー → 配列の要素(a,b,c)
    static public int []  Key2Hangulnumber( string keystring ) {
        int [] keyindex = new int [3]; 
        
        // 先頭子音のチェック -----------------
        int consonatNum=-1;
        for(int i=0; i < ConsonantKeyList.Length; i++ ){
            if(keystring.Substring(0,1) == ConsonantKeyList[i] ){
                consonatNum = i;
            }   
        }
        if(consonatNum != -1){
            // 先頭が子音で始まっていたら先頭子音削除
            keystring = keystring.Remove(0, 1);
        }

        // 母音のチェック -----------------
        int vowelNum=-1;
        int vowelLength = 0;
        for(int i=0; i < VowelKeyList.Length; i++ ){
            if(keystring.StartsWith(VowelKeyList[i])){
                vowelNum=i;
                vowelLength = VowelKeyList[i].Length;
                if(vowelLength==2){
                    break;
                }
            }
        }
        if(vowelNum != -1){
            keystring = keystring.Remove(0, vowelLength);
        }
        
        // Pachimのチェック -----------------
        int pachimNum=-1;
        for(int i=0; i < PachimKeyList.Length; i++ ){
            if(keystring == PachimKeyList[i]){
                pachimNum=i;
                if(PachimList[i].Length==2){
                    break;
                }
            }
        }
        
        // マッチしなかったら -1
        keyindex[0] = consonatNum;
        keyindex[1] = vowelNum;
        keyindex[2] = pachimNum;
        
        return ( keyindex ) ;
    }
    
    //------------------------------------------------------------------------
    // Hangul  --->  2bolsik hangul key
    //  ex. 라   ---> fk    
    //      락   ---> fkr   
    //      라가 ---> fkrk
    static public string  Hangul2KeyString( string hangul  ) {
        string keystring  = "";  // 変換したキー列
        
        for (int cnt=0; cnt < hangul.Length ;  cnt++ )
        {
            char ch = hangul[cnt];
            string alpha = HangulChar2Key(ch);

            keystring += alpha;
        }
        
        return  keystring; 
    }
    //------------------------------------------------------------------------
    static string key2lower(string keystring){
        string lowersting="";
        if(keystring==null){ return "";}
        
        for(int i=0; i < keystring.Length; i++) {
            string key = keystring.Substring(i,1);
            
            if (key == "Y") { lowersting += "y"; continue; }
            if (key == "U") { lowersting += "u"; continue; }
            if (key == "I") { lowersting += "i"; continue; }
                                                          
            if (key == "A") { lowersting += "a"; continue; }  
            if (key == "S") { lowersting += "s"; continue; }
            if (key == "D") { lowersting += "d"; continue; }
            if (key == "F") { lowersting += "f"; continue; } 
            if (key == "G") { lowersting += "h"; continue; }
                                                          
            if (key == "H") { lowersting += "h"; continue; }
            if (key == "J") { lowersting += "j"; continue; }
            if (key == "K") { lowersting += "k"; continue; }
            if (key == "L") { lowersting += "l"; continue; }
                                                          
            if (key == "Z") { lowersting += "k"; continue; }
            if (key == "X") { lowersting += "t"; continue; }
            if (key == "C") { lowersting += "c"; continue; }
            if (key == "V") { lowersting += "v"; continue; }
                                                          
            if (key == "B") { lowersting += "b"; continue; }
            if (key == "N") { lowersting += "n"; continue; }
            if (key == "M") { lowersting += "m"; continue; }
                                                 
            lowersting += key;
        }
        
        return lowersting ;
    }
    // 今は使っていない
    static private string  Key2Jamo( string key ) {
        
        // consonat
        if (key == "q") { return("B"); }
        if (key == "w") { return("J"); }
        if (key == "e") { return("D"); }
        if (key == "r") { return("G"); }
        if (key == "t") { return("S"); }
        
        if (key == "a") { return("M"); }
        if (key == "s") { return("N"); }
        if (key == "d") { return("");  }
        if (key == "f") { return("R"); } 
        if (key == "g") { return("H"); }
        
        if (key == "z") { return("K"); }
        if (key == "x") { return("T"); }
        if (key == "c") { return("C"); }
        if (key == "v") { return("P"); }

        if (key == "Q") { return("BB"); }  // 変わる
        if (key == "W") { return("JJ"); }  // 変わる
        if (key == "E") { return("DD"); }  // 変わる
        if (key == "R") { return("GG"); }  // 変わる
        if (key == "T") { return("SS"); }  // 変わる
        
        if (key == "A") { return("M"); }  
        if (key == "S") { return("N"); }
        if (key == "D") { return("");  }
        if (key == "F") { return("R"); } 
        if (key == "G") { return("H"); }
        
        if (key == "Z") { return("K"); }
        if (key == "X") { return("T"); }
        if (key == "C") { return("C"); }
        if (key == "V") { return("P"); }

        // vowel
        if (key == "y") { return("YO"); }
        if (key == "u") { return("YEO");  }
        if (key == "i") { return("YA");  }
        if (key == "o") { return("AE");  }
        if (key == "p") { return("E");  }
        
        if (key == "h") { return("O");  }
        if (key == "j") { return("EO"); }
        if (key == "k") { return("A");  }
        if (key == "l") { return("I");  }

        if (key == "b") { return("YU"); }
        if (key == "n") { return("U");  }
        if (key == "m") { return("EU"); }
        
        if (key == "Y") { return("YO"); }
        if (key == "U") { return("YEO");  }
        if (key == "I") { return("YA");  }
        if (key == "O") { return("YAE");  }  // 変わる
        if (key == "P") { return("YE");  }   // 変わる

        if (key == "H") { return("O");  }
        if (key == "J") { return("EO"); }
        if (key == "K") { return("A");  }
        if (key == "L") { return("I");  }

        if (key == "B") { return("YU"); }
        if (key == "N") { return("U");  }
        if (key == "M") { return("EU"); }
                                             
        return("");
    }
    //------------------------------------------------------------------------
    static private string  ConvertLastConsonat( string kRoman ) {

        if(kRoman.EndsWith("R") ||
           kRoman.EndsWith("M")      ){
            kRoman += "U";
            return kRoman;
        }
        if(kRoman.EndsWith("G")  &&  !kRoman.EndsWith("NG")     ){
            kRoman = kRoman.Remove(kRoman.Length-1, 1) + "KU";
            return kRoman;
        }
        if(kRoman.EndsWith("B")  ){
            kRoman = kRoman.Remove(kRoman.Length-1, 1) + "MU";
            return kRoman;
        }
        
        if(kRoman.EndsWith("SS")  ){
            kRoman=kRoman.Replace("SS", "ッ");
            return kRoman;
        }
        if(kRoman.EndsWith("S")  ||
           kRoman.EndsWith("J")  || 
           kRoman.EndsWith("C")     ){
            kRoman = kRoman.Remove(kRoman.Length-1, 1) +  "ッ" ;
            return kRoman;
        }
        
        return kRoman ;
    }
    //------------------------------------------------------------------------
    // input         return       rest
    // KAM   ------>    KA    +    M
    
    static private string  RemoveConsonat( string kRoman , out string rest) {
        rest="";

        if(isEndsWithConsonant(kRoman)){
            rest = kRoman.Substring(kRoman.Length-1, 1);
            kRoman = kRoman.Remove(kRoman.Length-1, 1) ;
        }

        return kRoman;
    }
    //------------------------------------------------------------------------
    // 
    static private string  ConvertVowel( string kRoman ) {
        
        kRoman=kRoman.Replace("EO", "O");
        kRoman=kRoman.Replace("OE", "E");
        kRoman=kRoman.Replace("AE", "E");
        kRoman=kRoman.Replace("EU", "U");

        return kRoman;
    }

    //------------------------------------------------------------------------
    // 清音化
    static private string  Geki2Hei_on_Head( string kRoman ) {
        
        // 激音==>平音
        if(kRoman.StartsWith("GG")){
            kRoman = "K" + kRoman.Remove(0, 2);
        }
        if(kRoman.StartsWith("DD")){
            kRoman = "T" + kRoman.Remove(0, 2);
        }
        if(kRoman.StartsWith("SS")){
            kRoman = "S" + kRoman.Remove(0, 2);
        }
        if(kRoman.StartsWith("JJ")){
            kRoman = "C" + kRoman.Remove(0, 2);
        }
        if(kRoman.StartsWith("BB")){
            kRoman = "P" + kRoman.Remove(0, 2);
        }
        return kRoman;
    }
        
        
    //------------------------------------------------------------------------
    // 先頭濁音の清音化
    static private string Dull2Resonant_on_Head ( string kRoman ) {
        if(kRoman.StartsWith("G") && !kRoman.StartsWith("GG") ){
            kRoman = "K" + kRoman.Remove(0, 1);
        }
        if(kRoman.StartsWith("B") && !kRoman.StartsWith("BB") ){
            kRoman = "P" + kRoman.Remove(0, 1);
        }
        if(kRoman.StartsWith("D") && !kRoman.StartsWith("DD") ){
            kRoman = "T" + kRoman.Remove(0, 1);
        }
        if(kRoman.StartsWith("J") && !kRoman.StartsWith("JJ") ){
            kRoman = "C" + kRoman.Remove(0, 1);
        }
        return kRoman;
    }


            

    //------------------------------------------------------------------------
    // 母音始まりか？
    static private bool isStartsWithVowel( string roman ) {
        if(roman.StartsWith("A") ||
           roman.StartsWith("I") ||
           roman.StartsWith("E") ||
           roman.StartsWith("U") ||
           roman.StartsWith("O") ||
           roman.StartsWith("W") ||
           roman.StartsWith("Y")     ){
            return true;
        }
        return false;
    }
    //------------------------------------------------------------------------
    // 子音終わりか？
    static private bool isEndsWithConsonant( string roman ) {
        if(roman.EndsWith("C") ||
           roman.EndsWith("D") ||
           roman.EndsWith("G") ||
           roman.EndsWith("H") ||
           roman.EndsWith("J") ||
           roman.EndsWith("K") ||
           roman.EndsWith("L") ||
           roman.EndsWith("M") ||
           roman.EndsWith("N") ||
           roman.EndsWith("P") ||
           roman.EndsWith("R") ||
           roman.EndsWith("S") ||
           roman.EndsWith("T")     ){
            return true;
        }
        return false;
    }
    
    //------------------------------------------------------------------------
    // 完全一致チェック
    //     "G","GG","N" などでのチェック(ハングルではない)
    static private bool isConsonant( string key ) {
        for(int i=0; i < ConsonantList.Length; i++ ){
            if(ConsonantList[i] == key){
                return true;
            }
        }
        
        return false;
    }
    static private bool isVowel( string key ) {
        for(int i=0; i < VowelList.Length; i++ ){
            if(VowelList[i] == key){
                return true;
            }
        }
        return false;
    }
    static private bool isPachim( string key ) {
        for(int i=0; i < PachimList.Length; i++ ){
            if(PachimList[i] == key){
                return true;
            }
        }
        return false;
    }
    //------------------------------------------------------------------------
    // "r"+"k" など2ボル式キーでの母音・子音判定
    static private bool isConsonantKey( string key ) {
        for(int i=0; i < ConsonantKeyList.Length; i++ ){
            if(ConsonantKeyList[i] == key){
                return true;
            }
        }
        
        return false;
    }
    static private bool isVowelKey( string key ) {
        for(int i=0; i < VowelKeyList.Length; i++ ){
            if(VowelKeyList[i] == key){
                return true;
            }
        }
        return false;
    }
    //------------------------------------------------------------------------
    
} // end of class Hangul



//-----------------------------------------------------------------------------
} // end of namespace  HYAM.Lingua

