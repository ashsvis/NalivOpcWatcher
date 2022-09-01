unit ApplicationUnit;

{$WARN SYMBOL_PLATFORM OFF}

interface

uses
  Windows, SysUtils, ComObj, ActiveX, AxCtrls, Classes, OpcBridgeServer_TLB,
  StdVcl, StrUtils;

type
  TApplication = class(TAutoObject, IConnectionPointContainer, IApplication)
  private
    { Private declarations }
    FConnectionPoints: TConnectionPoints;
    FConnectionPoint: TConnectionPoint;
    FEvents: IApplicationEvents;
    { note: FEvents maintains a *single* event sink. For access to more
      than one event sink, use FConnectionPoint.SinkList, and iterate
      through the list of sinks. }
  public
    procedure Initialize; override;
  protected
    { Protected declarations }
    property ConnectionPoints: TConnectionPoints read FConnectionPoints
      implements IConnectionPointContainer;
    procedure EventSinkChanged(const EventSink: IUnknown); override;
    procedure InitOPC; safecall;
    procedure FinitOPC; safecall;
    function AddServer(const ServerName: WideString): Integer; safecall;
    procedure RemoveServer(const ServerName: WideString); safecall;
    function AddGroup(const ServerName, GroupName: WideString): Integer;
      safecall;
    procedure RemoveGroup(const ServerName, GroupName: WideString); safecall;
    function AddItem(const ServerName, GroupName,
      ItemName: WideString): Integer; safecall;
    procedure RemoveItem(const ServerName, GroupName, ItemName: WideString);
      safecall;
    function FetchItem(const ServerName, GroupName,
      ItemName: WideString): WideString; safecall;
    function GetServers: WideString; safecall;
    function GetProps(const Server: WideString): WideString; safecall;
  end;

implementation

uses ComCtrls, ComServ, OPCtypes, OPCDA, OPCenum, OPCutils;

const
  RPC_C_AUTHN_LEVEL_NONE = 1;
  RPC_C_IMP_LEVEL_IMPERSONATE = 3;
  EOAC_NONE = 0;
  INVALIDOPCHANDLE = -2147023174;

var
  Enabled: boolean;
  ServerNames: TStringList;
  Servers: TInterfaceList;
  GroupNames: TStringList;
  Groups: TInterfaceList;
  GroupHandles: TList;
  ItemNames: TStringList;
  ItemsTypeCDT: TList;
  ItemHandles: TList;
//------------------------------

procedure TApplication.EventSinkChanged(const EventSink: IUnknown);
begin
  FEvents := EventSink as IApplicationEvents;
end;

procedure TApplication.Initialize;
begin
  inherited Initialize;
  FConnectionPoints := TConnectionPoints.Create(Self);
  if AutoFactory.EventTypeInfo <> nil then
    FConnectionPoint := FConnectionPoints.CreateConnectionPoint(
      AutoFactory.EventIID, ckSingle, EventConnect)
  else FConnectionPoint := nil;
end;

procedure TApplication.InitOPC;
begin
  // this is for DCOM:
  // without this, callbacks from the server may get blocked, depending on
  // DCOM configuration settings
  CoInitializeSecurity(
    nil,                    // points to security descriptor
    -1,                     // count of entries in asAuthSvc
    nil,                    // array of names to register
    nil,                    // reserved for future use
    RPC_C_AUTHN_LEVEL_NONE, // the default authentication level for proxies
    RPC_C_IMP_LEVEL_IMPERSONATE,// the default impersonation level for proxies
    nil,                    // used only on Windows 2000
    EOAC_NONE,              // additional client or server-side capabilities
    nil                     // reserved for future use
    );
  ServerNames := TStringList.Create;
  Servers := TInterfaceList.Create;
  GroupNames := TStringList.Create;
  Groups := TInterfaceList.Create;
  GroupHandles := TList.Create;
  ItemNames := TStringList.Create;
  ItemsTypeCDT := TList.Create;
  ItemHandles := TList.Create;
  Enabled := True; //not Failed(HR); // Failed to initialize DCOM security
end;

procedure TApplication.FinitOPC;
var i, j, k: Integer; ServerName, GroupName, ItemName: string;
begin
  for i := ServerNames.Count - 1 downto 0 do
  begin
    ServerName := ServerNames[i];
    for j := GroupNames.Count - 1 downto 0 do
    begin
      GroupName := GroupNames[j];
      Delete(GroupName, 1, Length(ServerName));
      for k := ItemNames.Count - 1 downto 0 do
      begin
        ItemName := ItemNames[k];
        Delete(ItemName, 1, Length(ServerName) + Length(GroupName));
        RemoveItem(ServerName, GroupName, ItemName);
      end;
      RemoveGroup(ServerName, PChar(GroupName));
    end;
    RemoveServer(ServerName);
  end;
  ItemsTypeCDT.Free;
  ItemHandles.Free;
  ItemNames.Free;
  GroupHandles.Free;
  Groups.Free;
  GroupNames.Free;
  Servers.Free;
  ServerNames.Free;
end;

function TApplication.AddServer(const ServerName: WideString): Integer;
var
  EnumOPC: IOPCServer;
begin
  if Enabled then
  begin
    Result := ServerNames.IndexOf(ServerName);
    if Result < 0 then
    begin
      // Connect OPC server
      try
        EnumOPC := CreateComObject(ProgIDToClassID(ServerName)) as IOPCServer;
        if EnumOPC <> nil then
          Result := ServerNames.AddObject(ServerName, TObject(Servers.Add(EnumOPC)));
      except
        Result := -1;
      end;
    end;
  end
  else
    Result := -1;
end;

procedure TApplication.RemoveServer(const ServerName: WideString);
var EnumOPC: IOPCServer; IndexServer: integer;
begin
  if Enabled then
  begin
    IndexServer := ServerNames.IndexOf(ServerName);
    if IndexServer >= 0 then
    begin
      IndexServer := Integer(ServerNames.Objects[IndexServer]);
      EnumOPC := Servers.Items[IndexServer] as IOPCServer;
      if EnumOPC <> nil then
      begin
        Servers.Items[IndexServer] := nil;
        Servers.Delete(IndexServer);
        ServerNames.Delete(IndexServer);
      end;
    end;
  end;
end;

function TApplication.AddGroup(const ServerName,
  GroupName: WideString): Integer;
var
  HR: HResult;
  EnumOPC: IOPCServer;
  IndexServer, IndexGroup: integer;
  ServerGroupName: string;
  GroupIf: IOPCItemMgt;
  GroupHandle: OPCHANDLE;
begin
  if Enabled then
  begin
    IndexServer := AddServer(ServerName);
    if IndexServer >= 0 then
    begin
      EnumOPC := Servers.Items[Integer(ServerNames.Objects[IndexServer])] as IOPCServer;
      if EnumOPC <> nil then
      begin
        ServerGroupName := ServerName + GroupName;
        IndexGroup := GroupNames.IndexOf(ServerGroupName);
        if IndexGroup < 0 then
        begin
          // now add an group to the OPC server
          HR := ServerAddGroup(EnumOPC, GroupName, True, 500, 0, GroupIf, GroupHandle);
          if Succeeded(HR) then
          begin
            IndexGroup := Groups.Add(GroupIf);
            GroupHandles.Add(Pointer(GroupHandle));
            GroupNames.AddObject(ServerGroupName, TObject(IndexGroup));
            Result := IndexGroup;
          end
          else
            Result := -1;
        end
        else
          Result := IndexGroup;
      end
      else
        Result := -1;
    end
    else
      Result := -1;
  end
  else
    Result := -1;
end;

procedure TApplication.RemoveGroup(const ServerName,
  GroupName: WideString);
var
  EnumOPC: IOPCServer; GroupHandle: OPCHANDLE;
  IndexServer, IndexGroup: integer;
  ServerGroupName: string;
begin
  if Enabled then
  begin
    IndexServer := ServerNames.IndexOf(ServerName);
    if IndexServer >= 0 then
    begin
      EnumOPC := Servers.Items[Integer(ServerNames.Objects[IndexServer])] as IOPCServer;
      if EnumOPC <> nil then
      begin
        ServerGroupName := ServerName + GroupName;
        IndexGroup := GroupNames.IndexOf(ServerGroupName);
        if IndexGroup >= 0 then
        begin
          GroupHandle := OPCHANDLE(GroupHandles[IndexGroup]);
          if GroupHandle <> 0 then
          begin
            EnumOPC.RemoveGroup(GroupHandle, False);
            GroupHandles.Delete(IndexGroup);
            Groups.Delete(IndexGroup);
            GroupNames.Delete(IndexGroup);
          end;
        end;
      end;
    end;
  end;
end;

function TApplication.AddItem(const ServerName, GroupName,
  ItemName: WideString): Integer;
var
  HR: HResult;
  GroupIf: IOPCItemMgt;
  ItemHandle: OPCHANDLE;
  ItemTypeCDT: TVarType;
  ServerGroupItemName: string;
  IndexGroup, IndexItem: integer;
begin
  if Enabled then
  begin
    IndexGroup := AddGroup(ServerName, GroupName);
    if IndexGroup >= 0 then
    begin
      GroupIf := Groups[IndexGroup] as IOPCItemMgt;
      if GroupIf <> nil then
      begin
        ServerGroupItemName := string(ServerName) + string(GroupName) + string(ItemName);
        IndexItem := ItemNames.IndexOf(ServerGroupItemName);
        if IndexItem < 0 then
        begin
          // now add an item to the group
          HR := GroupAddItem(GroupIf, ItemName, 0, VT_EMPTY, ItemHandle, ItemTypeCDT);
          if Succeeded(HR) then
          begin
            IndexItem := ItemHandles.Add(Pointer(ItemHandle));
            ItemsTypeCDT.Add(Pointer(ItemTypeCDT));
            ItemNames.AddObject(ServerGroupItemName, TObject(IndexItem));
            Result := IndexItem;
          end
          else
            Result := -1;
        end
        else
          Result := IndexItem;
      end
      else
        Result := -1;
    end
    else
      Result := -1;
  end
  else
    Result := -1;
end;

procedure TApplication.RemoveItem(const ServerName, GroupName,
  ItemName: WideString);
var
  GroupIf: IOPCItemMgt; ItemHandle: OPCHANDLE;
  GroupIndex, ItemIndex: Integer;
begin
  if Enabled then
  begin
    GroupIndex := GroupNames.IndexOf(string(ServerName) + string(GroupName));
    if GroupIndex >= 0 then
    begin
      GroupIf := Groups[GroupIndex] as IOPCItemMgt;
      if GroupIf <> nil then
      begin
        ItemIndex := ItemNames.IndexOf(string(ServerName) + string(GroupName) + string(ItemName));
        if ItemIndex >= 0 then
        begin
          ItemHandle := OPCHANDLE(ItemHandles[ItemIndex]);
          if ItemHandle <> 0 then
          begin
            GroupRemoveItem(GroupIf, ItemHandle);
            ItemsTypeCDT.Delete(ItemIndex);
            ItemHandles.Delete(ItemIndex);
            ItemNames.Delete(ItemIndex);
          end;
        end;
      end;
    end;
  end;
end;

function TApplication.FetchItem(const ServerName, GroupName,
  ItemName: WideString): WideString;
var
  HR: HResult;
  GroupIf: IOPCItemMgt; ItemHandle: OPCHANDLE;
  ItemValue: string; ItemQuality: Word; ItemTime: TFileTime;
  GroupIndex, ItemIndex: integer;
  Quality: string;
  SystemTime: TSystemTime;
begin
  Result := '0;BAD;00:00:00';
  if Enabled then
  begin
    GroupIndex := AddGroup(ServerName, GroupName);
    if GroupIndex >= 0 then
    begin
      GroupIf := Groups[GroupIndex] as IOPCItemMgt;
      if GroupIf <> nil then
      begin
        ItemIndex := AddItem(ServerName, GroupName, ItemName);
        if ItemIndex >= 0 then
        begin
          ItemHandle := OPCHANDLE(ItemHandles[ItemIndex]);
          if ItemHandle <> 0 then
          begin
            // now try to read the item value synchronously
            HR := ReadOPCGroupItemValue(GroupIf, ItemHandle, ItemValue,
                                        ItemQuality, ItemTime);
            if Succeeded(HR) then
            begin
              case ItemQuality and OPC_QUALITY_MASK of
                OPC_QUALITY_GOOD:      Quality := 'GOOD';
                OPC_QUALITY_UNCERTAIN: Quality := 'UNCERTAIN';
                else
                  Quality := 'BAD';
              end;
              FileTimeToSystemTime(ItemTime, SystemTime);
              Result := ItemValue + ';' + Quality + ';' +
                             FormatDateTime('dd.mm.yyyy hh:nn:ss.zzz',
                                            SystemTimeToDateTime(SystemTime));
            end;
          end;
        end;
      end;
    end;
  end;
end;

function TApplication.GetServers: WideString;
var
  OPCServerList: TOPCServerList;
  CATIDs: array of TGUID;
  L: TStringList; i: integer;
  function AddListOfServers(G: TGUID; Name: string): string;
  var L: TStringList; i: integer;
  begin
    Result := '';
    SetLength(CATIDs, 1);
    CATIDs[0] := G;
    try
      L := TStringList.Create;
      OPCServerList := TOPCServerList.Create('', False, CATIDs);
      try
        OPCServerList.Update;
        L.Assign(OPCServerList.Items);
        L.Sort;
        for i := 0 to L.Count - 1 do Result := Result + Name + '=' + L[i] + ';';
      finally
        OPCServerList.Free;
        L.Free;
      end;
    except
      Result := '';
    end;
  end;
begin
  Result := '';
  SetLength(CATIDs, 0);
  try
    L := TStringList.Create;
    OPCServerList := TOPCServerList.Create('', True, CATIDs);
    try
      OPCServerList.Update;
      L.Assign(OPCServerList.Items);
      L.Sort;
      for i := 0 to L.Count - 1 do Result := Result + 'Registry=' + L[i] + ';';
    finally
      OPCServerList.Free;
      L.Free;
    end;
  except
    Result := '';
  end;
  Result := Result + AddListOfServers(CATID_OPCDAServer10, 'OPC DA1.0');
  Result := Result + AddListOfServers(CATID_OPCDAServer20, 'OPC DA2.0');
  Result := Result + AddListOfServers(CATID_OPCDAServer30, 'OPC DA3.0');
  Result := Result + AddListOfServers(CATID_XMLDAServer10, 'XML DA1.0');
end;

function TApplication.GetProps(const Server: WideString): WideString;
var
  i: integer;
  Tags: TStringList;
  EnumOPC: IOPCServer;
  EnumProperties: IOPCBrowseServerAddressSpace;
  Alloc: IMalloc;
  NST: OPCNAMESPACETYPE;
  HR: HResult;
  EnumString: IEnumString;
  Res: PWideChar;
  VarType: TVarType;
  function StrToVarType(Value: WideString): TVarType;
  begin
    if Value = 'string' then
      Result := varOleStr
    else if Value = 'integer' then
      Result := varInteger
    else if Value = 'double' then
      Result := varDouble
    else
      Result := varEmpty;
  end;
  function Clear(value: string): string;
  begin
    Result := StrUtils.AnsiReplaceStr(value,#13#10,'');
  end;
  function UseCDT(EnumOPC: IOPCServer; ItemId: string): TVarType;
  var
    HR: HResult;
    PropertyInfo: IOPCItemProperties;
    pdwCount: Cardinal;
    ppPropertyIDs: PDWORDARRAY;
    ppDescriptions: POleStrList;
    ppvtDataTypes: PVarTypeList;
    ppvData: POleVariantArray;
    ppErrors: PResultList;
    I: Integer;
  begin
    Result := VT_EMPTY;
    PropertyInfo := EnumOPC as IOPCItemProperties;
    if PropertyInfo <> nil then
    begin
      HR := PropertyInfo.QueryAvailableProperties(
                                            PWideChar(WideString(ItemId)),
                                            pdwCount, ppPropertyIDs,
                                            ppDescriptions, ppvtDataTypes);
      if Succeeded(HR) then
      begin
        HR := PropertyInfo.GetItemProperties(PWideChar(WideString(ItemId)),
                                        pdwCount, ppPropertyIDs,
                                        ppvData, ppErrors);
        if Succeeded(HR) then
        begin
          for I := 0 to pdwCount - 1 do
          if ppPropertyIDs <> nil then
          begin
            if (ppvData <> nil) then
            begin
              if ppPropertyIDs[i] = OPC_PROP_CDT then
              begin
                //расшифровка канонического типа данных
                try
                  Result := ppvData[i];
                except
                  Result := StrToVarType(ppvData[i]);
                end;
                Break;
              end;
            end;
          end;
        end;
      end;
    end;
  end;
  procedure BrowseBranch(EnumProperties: IOPCBrowseServerAddressSpace);
  var
    EnumString: IEnumString;
    HR: HResult;
    BrowseID: PWideChar;
    aItemID: PWideChar;
    celtFetched: LongInt;
  begin
    HR := EnumProperties.BrowseOpcItemIds(OPC_LEAF, '', VT_EMPTY, 0, EnumString);
    if HR <> S_FALSE then
    begin
      OleCheck(HR);
      while EnumString.Next(1, BrowseID, @celtFetched) = S_OK do
      begin
        OleCheck(EnumProperties.GetItemID(BrowseID, aItemID));
        VarType := UseCDT(EnumOPC, aItemID);
        Tags.Append(Clear(string(aItemID)) + '=' + IntToStr(VarType));
        Alloc.Free(aItemID);
        Alloc.Free(BrowseID);
      end
    end;
    HR := EnumProperties.BrowseOpcItemIds(OPC_BRANCH, '', VT_EMPTY, 0, EnumString);
    if HR <> S_FALSE then
    begin
      OleCheck(HR);
      while EnumString.Next(1, BrowseID, nil) = S_OK do
      begin
        OleCheck(EnumProperties.ChangeBrowsePosition(OPC_BROWSE_DOWN, BrowseID));
        Alloc.Free(BrowseID);
        BrowseBranch(EnumProperties);
        OleCheck(EnumProperties.ChangeBrowsePosition(OPC_BROWSE_UP, ''));
      end
    end
  end;
begin
  Result := '';
  i := AddServer(Server);
  if i < 0 then Exit;
  EnumOPC := Servers[i] as IOPCServer;
  EnumProperties := EnumOPC as IOPCBrowseServerAddressSpace;
  CoGetMalloc(1, Alloc);
  // Получение типа дерева в переменную NST: .._FLAT - список, иначе дерево.
  OleCheck(EnumProperties.QueryOrganization(NST));
  Tags := TStringList.Create;
  try
    if NST = OPC_NS_FLAT then
    begin // обработка списка
      HR := EnumProperties.BrowseOpcItemIds(OPC_FLAT, '', VT_EMPTY, 0, EnumString);
      if HR <> S_FALSE then
      begin
        OleCheck(HR);
        while EnumString.Next(1, Res, nil) = S_OK do
        begin
          VarType := UseCDT(EnumOPC, Res);
          Tags.Append(Clear(string(Res)) + '=' + IntToStr(VarType));
          Alloc.Free(Res);
        end
      end
    end
    else
    begin // обработка дерева
      HR := EnumProperties.ChangeBrowsePosition(OPC_BROWSE_TO, '');
      if HR = E_INVALIDARG then
        repeat
          HR := EnumProperties.ChangeBrowsePosition(OPC_BROWSE_UP, '')
        until HR <> S_OK
      else
        OleCheck(HR);
      BrowseBranch(EnumProperties);
    end;
    Tags.Sort;
    Tags.Delimiter := ';';
    Result := Tags.DelimitedText;
  finally
    Tags.Free;
  end;
end;

initialization
  TAutoObjectFactory.Create(ComServer, TApplication, Class_Application,
    ciMultiInstance, tmApartment);
end.
