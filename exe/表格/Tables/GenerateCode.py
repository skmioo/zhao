#!/usr/bin/python
#-*- coding: utf-8 -*-

#生成文件测试

import shutil
import os
import sys
import string



##--------------------------探索表并填入各字段信息----------------------------
def FindTableFileInfo(tableFileName,types,variables,comments):
    #tableFileName = "achievement.txt"
    tableFile = open(tableFileName,"r")
    lineType = tableFile.readline()
    lineVariable = tableFile.readline()
    lineComment = tableFile.readline()

    typeStrings = lineType.split("\t")
    variableStrings = lineVariable.split("\t")
    commentStrings = lineComment.split("\t")

    #填入表中所有字段类型
    for typeString in typeStrings:
        if typeString.lower().startswith("int"):
            type = "int"
        elif typeString.lower().startswith("float"):
            type = "float"
        elif typeString.lower().startswith("bool"):
            type = "bool"
        elif typeString.lower().startswith("string"):
                type = "string"
        else:
            continue
        if typeString.find("array") >=0:
            type = type + "[]"

        types.append(type)

    #填入表中所有字段名称
    for variableString in variableStrings:
        if variableString.lstrip() != "":
            variables.append(variableString.lstrip().rstrip())

    #填入表中所有字段注释
    for commentString in commentStrings:
        if commentString.lstrip() != "":
            comments.append(commentString.lstrip().rstrip())


    #print len(types)
    #for type in types:
    #    print type

    #for variable in variables:
    #    print variable

    #for comment in comments:
    #    print comment

    tableFile.close()
##--------------------------探索表并填入各字段信息结束----------------------------

	

##--------------------------生成className.cs----------------------------
def GenerateCSharpFile(className,types,variables,comments):
    #className = "Table_Achievement"
    cSharpFileName = "./Tables/" + className + ".cs"
    cSharpFile = open(cSharpFileName,"wb")

    #命名空间
    cSharpFile.write("using System;\n")
    cSharpFile.write("using System.Collections.Generic;\n")
    cSharpFile.write("using System.Linq;\n")
    cSharpFile.write("using System.Text;\n")
    cSharpFile.write("\n")

    #类
    cSharpFile.write("public class %s\n" % (className))
    cSharpFile.write("{\n")

    for i in range(0,len(types)):
        #注释
        cSharpFile.write("\t/// <summary>\n")
        cSharpFile.write("\t///%s\n" % (comments[i]))
        #cSharpFile.write("\t///暂无注释\n")
        cSharpFile.write("\t/// <summary>\n")
        #变量
        cSharpFile.write("\t[ColumnMapping(\"%s\")]\n" % (variables[i]))
        cSharpFile.write("\t\tpublic %s %s;\n" % (types[i],variables[i]))

    #ToString方法
    cSharpFile.write("\n\tpublic override string ToString()\n")
    cSharpFile.write("\t{\n")
    cSharpFile.write("\t\treturn base.ToString() + \": [ \"");

    for i in range(0,len(types)):
            cSharpFile.write(" + \" (%s : \" + %s + \")\"" % (variables[i],variables[i]))

    cSharpFile.write(" + \" ]\";\n");
    cSharpFile.write("\t}\n")


    cSharpFile.write("}\n")
    cSharpFile.close()
##--------------------------生成className.cs结束----------------------------

##--------------------------生成TableCtrl.cs----------------------------
def GenerateTableCtrl(classNames):
    cSharpFileName = "./TableCtrl.cs"
    cSharpFile = open(cSharpFileName,"wb")
    
    #命名空间
    cSharpFile.write("using System;\n")
    cSharpFile.write("using System.Collections.Generic;\n")
    cSharpFile.write("using System.IO;\n")
    cSharpFile.write("using UnityEngine;\n")
    cSharpFile.write("\n")

    #类
    cSharpFile.write("public class TableCtrl : CSharpSingletion<TableCtrl>\n")
    cSharpFile.write("{\n")
    #字段
    cSharpFile.write("\t#region 常量与字段\n")
    cSharpFile.write("\tprivate static TableCtrl _instance;")
    
    for i in range(0,len(classNames)):
        cSharpFile.write("\tpublic readonly Dictionary<string, %s> %sDic = new Dictionary<string, %s>();\n" % (classNames[i],classNames[i],classNames[i]))
    
    cSharpFile.write("\n\tpublic bool IsInitOk = false;\n")
    cSharpFile.write("\t#endregion\n")
    #方法
    cSharpFile.write("\n\t#region 方法\n")
    cSharpFile.write("\tpublic void OnInit()\n")
    cSharpFile.write("\t{\n")
    cSharpFile.write("\t\tInit(\"Tables/Txt/\");\n")
    cSharpFile.write("\t}\n\n")
    cSharpFile.write("\tpublic void Init(string url)\n")
    cSharpFile.write("\t{\n")
    cSharpFile.write("\t\tDictionary<string, byte[]> data = new Dictionary<string, byte[]>();\n")
    cSharpFile.write("\t\tif (Directory.Exists(url))\n")
    cSharpFile.write("\t\t{\n")
    cSharpFile.write("\t\t\tDirectoryInfo direction = new DirectoryInfo(url);\n")
    cSharpFile.write("\t\t\tFileInfo[] files = direction.GetFiles(\"*\", SearchOption.AllDirectories);\n")
    cSharpFile.write("\t\t\tint len = files.Length;\n")
    cSharpFile.write("\t\t\tif (len == 0)\n")
    cSharpFile.write("\t\t\t{\n")
    cSharpFile.write("\t\t\t\tDebug.LogError(\"ERROR PATH[\" + url + \"]\");\n")
    cSharpFile.write("\t\t\t\treturn;\n")
    cSharpFile.write("\t\t\t}\n\n")
    cSharpFile.write("\t\t\tfor (int i = 0; i < files.Length; i++)\n")
    cSharpFile.write("\t\t\t{\n")
    cSharpFile.write("\t\t\t\tif (files[i].Name.EndsWith(\".meta\"))\n")
    cSharpFile.write("\t\t\t\t{\n")
    cSharpFile.write("\t\t\t\t\tcontinue;\n")
    cSharpFile.write("\t\t\t\t}\n")
    cSharpFile.write("\t\t\t\tdata.Add(files[i].Name.Split('.')[0].ToLower(), AuthGetFileData(files[i].DirectoryName + \"/\" + files[i].Name));\n")
    cSharpFile.write("\t\t\t}\n\n")
    cSharpFile.write("\t\t\tTableCtrl.Instance.ParseData(data);\n\n")
    cSharpFile.write("\t\t}\n")
    cSharpFile.write("\t\tIsInitOk = true;\n")
    cSharpFile.write("\t}\n\n")

    cSharpFile.write("\tpublic byte[] AuthGetFileData(string fileUrl)\n")
    cSharpFile.write("\t{\n")
    cSharpFile.write("\t\tFileStream fs = new FileStream(fileUrl, FileMode.Open, FileAccess.Read);\n")
    cSharpFile.write("\t\tbyte[] buffur = new byte[fs.Length];\n")
    cSharpFile.write("\t\tfs.Read(buffur, 0, buffur.Length);\n")
    cSharpFile.write("\t\tfs.Close();\n")
    cSharpFile.write("\t\treturn buffur;\n")
    cSharpFile.write("\t}\n")


    cSharpFile.write("\tprivate void ParseDataToVO<T>(Dictionary<string, T> dic, Dictionary<string, byte[]> data)\n")
    cSharpFile.write("\t{\n")
    cSharpFile.write("\t\tstring name =  typeof(T).ToString().ToLower().Replace(\"table_\",\"\");\n")
    cSharpFile.write("\t\ttry\n")
    cSharpFile.write("\t\t{\n")
    cSharpFile.write("\t\tbyte[] vos = null;\n")
    cSharpFile.write("\t\tdata.TryGetValue(name, out vos);\n")
    cSharpFile.write("\t\tif (null == vos)\n")
    cSharpFile.write("\t\t{\n")
    cSharpFile.write("\t\t\tDebug.LogError(\"table\" + name +\"is TryGetValue  ERROR !\");\n")
    cSharpFile.write("\t\t\treturn;\n")
    cSharpFile.write("\t\t}\n")
    cSharpFile.write("\t\tT[]  tarr = CsvImporter.Parser<T>(vos);\n\n")
    cSharpFile.write("\t\tstring id = \"\";\n")
    cSharpFile.write("\t\tT vo;\n")
    cSharpFile.write("\t\tfor (int i = 0, count = tarr.Length; i < count; i++)\n")
    cSharpFile.write("\t\t{\n")
    cSharpFile.write("\t\t\tvo = tarr[i];\n")
    cSharpFile.write("\t\t\tid = ObjUtil.GetValue(vo, \"id\").ToString();\n\n")
    cSharpFile.write("\t\t\tif (!dic.ContainsKey(id + \"\"))\n")
    cSharpFile.write("\t\t\t{\n")
    cSharpFile.write("\t\t\t\tdic.Add(id + \"\", vo);\n")
    cSharpFile.write("\t\t\t\tcontinue;\n")
    cSharpFile.write("\t\t\t}\n")
    cSharpFile.write("\t\t\tDebug.Log(name + \" table.ID \" + id +\" is duplicated!\");\n")
    cSharpFile.write("\t\t}\n")
    cSharpFile.write("\t\t}catch(Exception e) {\n")
    cSharpFile.write("\t\tDebug.LogError(\"=存在问题的配置表 ：\" + name );\n")
    cSharpFile.write("\t\t}\n")
    cSharpFile.write("\t}\n\n")


    cSharpFile.write("\tpublic void  ParseData(object obj)\n")
    cSharpFile.write("\t{\n")
    cSharpFile.write("\t\tDictionary<string, byte[]> data = obj as Dictionary<string, byte[]>;\n")

    for i in range(0,len(classNames)):
        cSharpFile.write("\t\tParseDataToVO<%s>(%sDic, data);\n" % (classNames[i],classNames[i]))

    cSharpFile.write("\n\t\tdata.Clear();\n")
    cSharpFile.write("\t\tdata = null;\n")
    cSharpFile.write("\t}\n\n")


    cSharpFile.write("\t#region 获取各表数据\n")

    for i in range(0,len(classNames)):
        cSharpFile.write("\tpublic %s Get%s(string key)\n" % (classNames[i],classNames[i]))
        cSharpFile.write("\t{\n")
        cSharpFile.write("\t\t%s tmp;\n" %(classNames[i]))
        cSharpFile.write("\t\tif (%sDic.TryGetValue(key, out tmp))\n" %(classNames[i]))
        cSharpFile.write("\t\t{\n")
        cSharpFile.write("\t\t\treturn tmp;\n")
        cSharpFile.write("\t\t}\n")
        cSharpFile.write("\t\treturn tmp;\n")
        cSharpFile.write("\t}\n\n")

    cSharpFile.write("\t#endregion\n")
    cSharpFile.write("\t#endregion\n")
    cSharpFile.write("}")


##--------------------------生成TableCtrl.cs结束----------------------------

if os.path.exists("Tables"):
    shutil.rmtree("Tables")
os.mkdir("Tables")

classNames = []

dir = "../Client/"
for filename in os.listdir(dir):
    temp = filename.split('.')
    if len(temp) != 2:
        continue
    name = temp[0]
    suffix = temp[1]
    if suffix!= "txt":
        print filename + "ignored..."
        continue
    types = []
    variables = []
    comments = []

    className = "Table_"+name.capitalize()
    classNames.append(className)

    FindTableFileInfo(dir+filename,types,variables,comments)
    GenerateCSharpFile(className,types,variables,comments)

GenerateTableCtrl(classNames)



