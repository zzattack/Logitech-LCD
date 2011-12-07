#include <Windows.h>
#include "lglcd.h"

__declspec (dllexport) DWORD Init(void)
{
	return lgLcdInit();
}

__declspec (dllexport)DWORD DeInit(void)
{
	return lgLcdDeInit();
}

__declspec (dllexport) DWORD Connect(lgLcdConnectContext *ctx)
{
	return lgLcdConnect(ctx);
}

__declspec (dllexport) DWORD ConnectEx(lgLcdConnectContextEx *ctx)
{
	return lgLcdConnectEx(ctx);
}

__declspec (dllexport) DWORD Disconnect(int connection)
{
	return lgLcdDisconnect(connection);
}

__declspec (dllexport) DWORD SetDeviceFamiliesToUse(
	int connection,
	DWORD dwDeviceFamiliesSupported,
	DWORD dwReserved1)
{
	return lgLcdSetDeviceFamiliesToUse(
		connection,
		dwDeviceFamiliesSupported,
		dwReserved1);
}

__declspec (dllexport) DWORD Enumerate(
	int connection,
	int index,
	lgLcdDeviceDesc *description)
{
	return lgLcdEnumerate(connection, index, description);
}

__declspec (dllexport) DWORD EnumerateEx(
	int connection,
	int index,
	lgLcdDeviceDescEx *description)
{
	return lgLcdEnumerateEx(connection, index, description);
}

__declspec (dllexport) DWORD Open(lgLcdOpenContext *ctx)
{
	return lgLcdOpen(ctx);
}

__declspec (dllexport) DWORD OpenByType(lgLcdOpenByTypeContext *ctx)
{
	return lgLcdOpenByType(ctx);
}

__declspec (dllexport) DWORD Close(int device)
{
	return lgLcdClose(device);
}

__declspec (dllexport) DWORD ReadSoftButtons(int device, DWORD *buttons)
{
	return lgLcdReadSoftButtons(device, buttons);
}

__declspec (dllexport) DWORD UpdateBitmap(
	int device,
	const lgLcdBitmapHeader *bitmap,
	DWORD priority)
{
	return lgLcdUpdateBitmap(device, bitmap, priority);
}

__declspec (dllexport) DWORD SetAsLCDForegroundApp(
	int device,
	int foregroundYesNoFlag)
{
	return lgLcdSetAsLCDForegroundApp(
		device,
		foregroundYesNoFlag);
}
