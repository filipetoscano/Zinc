@echo off

..\..\src\Zinc.WebServices.ProxyGenerator\bin\Debug\znproxy.exe ^
    --assembly=..\Zn.Sample\bin\Zn.Sample.dll ^
    --application=Sample ^
    --namespace=Zn.Sample ^
    --output=Proxy.cs
