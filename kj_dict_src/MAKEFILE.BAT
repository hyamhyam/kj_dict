@echo off
setlocal

cd _KJ_Linguadll || exit /b
csc /out:KJ_Lingua.dll /t:library *.cs ..\KJ_AssemblyInfo.cs  
move /y KJ_Lingua.dll ..
cd ..


cd _KJ_dictdll    || exit /b
csc /out:KJ_dict.dll /t:library KJ_*.cs ..\KJ_AssemblyInfo.cs  /r:..\KJ_Lingua.dll 
move /y KJ_dict.dll ..
cd ..


cd _KJ_Messagedll        || exit /b
csc /nowarn:168 /out:KJ_Message.dll /t:library KJ_*.cs ..\KJ_AssemblyInfo.cs /r:..\KJ_dict.dll 
move /y KJ_Message.dll ..
cd ..


cd _KJ_utildll      || exit /b
csc /nowarn:168 /out:KJ_util.dll /t:library KJ_*.cs ..\KJ_AssemblyInfo.cs /r:..\KJ_dict.dll  /r:..\KJ_Message.dll
move /y KJ_util.dll ..
cd ..



resgen KJ_Message.ja.txt KJ_Message.ja.resources
al /nologo /out:ja\KJ_Message.resources.dll /c:ja /embed:KJ_Message.ja.resources

resgen KJ_Message.en.txt KJ_Message.en.resources
al /nologo /out:en\KJ_Message.resources.dll /c:en /embed:KJ_Message.en.resources

resgen KJ_Message.ko.txt KJ_Message.ko.resources
al /nologo /out:ko\KJ_Message.resources.dll /c:ko /embed:KJ_Message.ko.resources





cd _KJ_form      || exit /b
set COMMONSRC=..\KJ_AssemblyInfo.cs 


set COMMONDLLS=/r:..\KJ_dict.dll /r:..\KJ_Lingua.dll /r:..\KJ_Message.dll /r:..\KJ_util.dll 

REM for VS2008
REM set COMMONDLLS=/r:..\KJ_dict.dll /r:..\KJ_Lingua.dll /r:..\KJ_Message.dll /r:..\KJ_util.dll /win32icon:..\book.ico


csc /t:winexe /out:KJ_dictform.exe KJ_dictform.cs KJ_dictform.Designer.cs KJ_dictform_setting.cs KJ_form_ext.cs KJ_hangul_button.cs KJ_hiragana_button.cs KJ_PosDetails.cs  KJ_hangul_keyboard.cs %COMMONSRC% %COMMONDLLS% 


move KJ_dictform.exe ..
REM  csc /t:winexe /out:KJ_form2.exe KJ_form2.cs %COMMONSRC% %COMMONDLLS% 
REM  move KJ_form2.exe ..

cd ..



endlocal
