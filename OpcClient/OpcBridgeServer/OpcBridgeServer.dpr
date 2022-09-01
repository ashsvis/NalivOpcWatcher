library OpcBridgeServer;

uses
  ComServ,
  OpcBridgeServer_TLB in 'OpcBridgeServer_TLB.pas',
  ApplicationUnit in 'ApplicationUnit.pas' {Application: CoClass},
  OPC_AE in 'OPC\OPC_AE.pas',
  OPCCOMN in 'OPC\OPCCOMN.pas',
  OPCDA in 'OPC\OPCDA.pas',
  OPCenum in 'OPC\OPCenum.pas',
  OpcError in 'OPC\OPCerror.pas',
  OPCHDA in 'OPC\OPCHDA.pas',
  OPCSEC in 'OPC\OPCSEC.pas',
  OPCtypes in 'OPC\OPCtypes.pas',
  OPCutils in 'OPC\OPCutils.pas';

exports
  DllGetClassObject,
  DllCanUnloadNow,
  DllRegisterServer,
  DllUnregisterServer;

{$R *.TLB}

{$R *.RES}

begin
end.
