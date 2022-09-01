unit OpcBridgeServer_TLB;

// ************************************************************************ //
// WARNING                                                                    
// -------                                                                    
// The types declared in this file were generated from data read from a       
// Type Library. If this type library is explicitly or indirectly (via        
// another type library referring to this type library) re-imported, or the   
// 'Refresh' command of the Type Library Editor activated while editing the   
// Type Library, the contents of this file will be regenerated and all        
// manual modifications will be lost.                                         
// ************************************************************************ //

// PASTLWTR : 1.2
// File generated on 28.03.2011 21:17:39 from Type Library described below.

// ************************************************************************  //
// Type Lib: D:\Work\OpcBridgeServer\OpcBridgeServer.tlb (1)
// LIBID: {C2450FD5-CB9A-4D41-B919-BFE7FB832143}
// LCID: 0
// Helpfile: 
// HelpString: OpcBridgeServer Library
// DepndLst: 
//   (1) v2.0 stdole, (C:\Windows\system32\stdole2.tlb)
// ************************************************************************ //
{$TYPEDADDRESS OFF} // Unit must be compiled without type-checked pointers. 
{$WARN SYMBOL_PLATFORM OFF}
{$WRITEABLECONST ON}
{$VARPROPSETTER ON}
interface

uses Windows, ActiveX, Classes, Graphics, StdVCL, Variants;
  

// *********************************************************************//
// GUIDS declared in the TypeLibrary. Following prefixes are used:        
//   Type Libraries     : LIBID_xxxx                                      
//   CoClasses          : CLASS_xxxx                                      
//   DISPInterfaces     : DIID_xxxx                                       
//   Non-DISP interfaces: IID_xxxx                                        
// *********************************************************************//
const
  // TypeLibrary Major and minor versions
  OpcBridgeServerMajorVersion = 1;
  OpcBridgeServerMinorVersion = 0;

  LIBID_OpcBridgeServer: TGUID = '{C2450FD5-CB9A-4D41-B919-BFE7FB832143}';

  IID_IApplication: TGUID = '{D1AE8111-5697-426E-BD03-AE67B3960233}';
  DIID_IApplicationEvents: TGUID = '{3774FE31-9A34-4DAE-846E-097C77A6C8F6}';
  CLASS_Application: TGUID = '{A8E1D51C-9C14-4398-A4A1-411E0A18D263}';
type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IApplication = interface;
  IApplicationDisp = dispinterface;
  IApplicationEvents = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  Application = IApplication;


// *********************************************************************//
// Interface: IApplication
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {D1AE8111-5697-426E-BD03-AE67B3960233}
// *********************************************************************//
  IApplication = interface(IDispatch)
    ['{D1AE8111-5697-426E-BD03-AE67B3960233}']
    procedure InitOPC; safecall;
    procedure FinitOPC; safecall;
    function AddServer(const ServerName: WideString): Integer; safecall;
    procedure RemoveServer(const ServerName: WideString); safecall;
    function AddGroup(const ServerName: WideString; const GroupName: WideString): Integer; safecall;
    procedure RemoveGroup(const ServerName: WideString; const GroupName: WideString); safecall;
    function AddItem(const ServerName: WideString; const GroupName: WideString; 
                     const ItemName: WideString): Integer; safecall;
    procedure RemoveItem(const ServerName: WideString; const GroupName: WideString; 
                         const ItemName: WideString); safecall;
    function FetchItem(const ServerName: WideString; const GroupName: WideString; 
                       const ItemName: WideString): WideString; safecall;
    function GetServers: WideString; safecall;
    function GetProps(const Server: WideString): WideString; safecall;
  end;

// *********************************************************************//
// DispIntf:  IApplicationDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {D1AE8111-5697-426E-BD03-AE67B3960233}
// *********************************************************************//
  IApplicationDisp = dispinterface
    ['{D1AE8111-5697-426E-BD03-AE67B3960233}']
    procedure InitOPC; dispid 201;
    procedure FinitOPC; dispid 202;
    function AddServer(const ServerName: WideString): Integer; dispid 203;
    procedure RemoveServer(const ServerName: WideString); dispid 204;
    function AddGroup(const ServerName: WideString; const GroupName: WideString): Integer; dispid 205;
    procedure RemoveGroup(const ServerName: WideString; const GroupName: WideString); dispid 206;
    function AddItem(const ServerName: WideString; const GroupName: WideString; 
                     const ItemName: WideString): Integer; dispid 207;
    procedure RemoveItem(const ServerName: WideString; const GroupName: WideString; 
                         const ItemName: WideString); dispid 208;
    function FetchItem(const ServerName: WideString; const GroupName: WideString; 
                       const ItemName: WideString): WideString; dispid 209;
    function GetServers: WideString; dispid 210;
    function GetProps(const Server: WideString): WideString; dispid 211;
  end;

// *********************************************************************//
// DispIntf:  IApplicationEvents
// Flags:     (4096) Dispatchable
// GUID:      {3774FE31-9A34-4DAE-846E-097C77A6C8F6}
// *********************************************************************//
  IApplicationEvents = dispinterface
    ['{3774FE31-9A34-4DAE-846E-097C77A6C8F6}']
  end;

// *********************************************************************//
// The Class CoApplication provides a Create and CreateRemote method to          
// create instances of the default interface IApplication exposed by              
// the CoClass Application. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoApplication = class
    class function Create: IApplication;
    class function CreateRemote(const MachineName: string): IApplication;
  end;

implementation

uses ComObj;

class function CoApplication.Create: IApplication;
begin
  Result := CreateComObject(CLASS_Application) as IApplication;
end;

class function CoApplication.CreateRemote(const MachineName: string): IApplication;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Application) as IApplication;
end;

end.
