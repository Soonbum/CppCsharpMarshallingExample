using Carima.File;
using static Carima.Device.Device.CyUSBSerial.CyUSBSerial;

namespace Carima.Device.Device.CyUSBSerial
{
    public class CyUSBSerialDevice
    {
#pragma warning disable CS0169, CS0414
        public const int VID = 0x04B4;
        public const int PID = 0x000A;

	    private CY_RETURN_STATUS cyReturnStatus;
        private CY_RETURN_STATUS cyReturnStatusA;
        private CY_RETURN_STATUS cyReturnStatusB;
        private CY_DEVICE_INFO cyDeviceInfo;
        private CY_DEVICE_INFO[] cyDeviceInfoList = new CY_DEVICE_INFO[16];
        private CY_VID_PID cyVidPid;
        private CY_I2C_CONFIG cyI2CConfig;
        private CY_I2C_DATA_CONFIG i2cDataConfig;
        private CY_DATA_BUFFER EngineA_cyDatabufferWrite, EngineA_cyDatabufferRead;
        private CY_DATA_BUFFER EngineB_cyDatabufferWrite, EngineB_cyDatabufferRead;
        private int EnableByte = 0;

        private byte index;
        private int Engine_Count = 0;

        private byte[] EngineA_wbuffer = new byte[7];
        private byte[] EngineA_rbuffer = new byte[7];
        private byte[] EngineB_wbuffer = new byte[7];
        private byte[] EngineB_rbuffer = new byte[7];

        private byte cyNumDevices = 0;
        private byte[] deviceID = new byte[16];
        private uint[] deviceArray = new uint[6];

        private byte GpioNum;
        private byte GpioValue;
#pragma warning restore CS0169, CS0414

        public void Load(ref CyUSBSerialControl deviceA, ref CyUSBSerialControl deviceB)
        {
            // Retrive the number of devices binded to USB Serial device driver    
            cyReturnStatus = CyGetListofDevices(ref cyNumDevices);
            cyReturnStatus = CyGetListofDevices(ref cyNumDevices);

            if (cyReturnStatus != CY_RETURN_STATUS.CY_SUCCESS)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CY:Error in Getting count of devices: Error NO:{0}  {1} device", cyReturnStatus, cyNumDevices);
                return;
            }

            // Retrive the device information each device binded to USB Serial device driver and display the information.
            if (CommonProperty.IsDebugMode) Console.WriteLine("===================CyGetDeviceInfo Begin===================");
            for (index = 0; index < cyNumDevices; index++)
            {
                cyReturnStatus = CyGetDeviceInfo(index, ref cyDeviceInfo);
                if (cyReturnStatus == CY_RETURN_STATUS.CY_SUCCESS)
                {
                    if (CommonProperty.IsDebugMode)
                    {
                        Console.WriteLine("CyGetDeviceInfo: {0}/{1}", index + 1, cyNumDevices);
                        Console.WriteLine("Number of interfaces: {0}\nVid: 0x{1:X} \nPid: 0x{2:X}\nSerial Number is: {3}\nManufacturer name: {4}\nProduct Name: {5}\nSCB Number: 0x{6:X}\nDevice Type: {7}\nDevice Class: {8}\n\n",
                            cyDeviceInfo.numInterfaces,
                            cyDeviceInfo.vidPid.vid,
                            cyDeviceInfo.vidPid.pid,
                            cyDeviceInfo.serialNum,
                            cyDeviceInfo.manufacturerName,
                            cyDeviceInfo.productName,
                            cyDeviceInfo.deviceBlock,
                            cyDeviceInfo.deviceType[0],
                            cyDeviceInfo.deviceClass[0]);
                    }
                }
            }

            if (CommonProperty.IsDebugMode) Console.WriteLine("===================CyGetDeviceInfo End===================");
            cyVidPid.vid = VID;
            cyVidPid.pid = PID;

            for (int i = 0; i < cyDeviceInfoList.Length; i++)
            {
                cyDeviceInfoList[i].vidPid.vid = 0;
                cyDeviceInfoList[i].vidPid.pid = 0;
                cyDeviceInfoList[i].numInterfaces = 0;
                cyDeviceInfoList[i].manufacturerName = new string('\0', CY_STRING_DESCRIPTOR_SIZE);
                cyDeviceInfoList[i].productName = new string('\0', CY_STRING_DESCRIPTOR_SIZE);
                cyDeviceInfoList[i].serialNum = new string('\0', CY_STRING_DESCRIPTOR_SIZE);
                cyDeviceInfoList[i].deviceFriendlyName = new string('\0', CY_STRING_DESCRIPTOR_SIZE);
                cyDeviceInfoList[i].deviceType = new CY_DEVICE_TYPE[CY_MAX_DEVICE_INTERFACE];
                cyDeviceInfoList[i].deviceClass = new CY_DEVICE_CLASS[CY_MAX_DEVICE_INTERFACE];
                cyDeviceInfoList[i].deviceBlock = CY_DEVICE_SERIAL_BLOCK.SerialBlock_MFG;
            }

            cyReturnStatus = CyGetDeviceInfoVidPid(cyVidPid, deviceID, cyDeviceInfoList, ref cyNumDevices, 16);

            int deviceIndexAtSCB0 = -1;
            int arrindex = 0;

            if (CommonProperty.IsDebugMode) Console.WriteLine("===================CyGetDeviceInfoVidPid Begin===================");
            for (int index = 0; index < cyNumDevices; index++)
            {
                // Find the device at device index at SCB0
                if (cyDeviceInfoList[index].deviceBlock == CY_DEVICE_SERIAL_BLOCK.SerialBlock_SCB0)
                {
                    if (CommonProperty.IsDebugMode)
                    {
                        Console.WriteLine("CyGetDeviceInfoVidPid: {0}/{1}", index + 1, cyNumDevices);
                        Console.WriteLine("Number of interfaces: {0}\nVid: 0x{1:X} \nPid: 0x{2:X}\nSerial Number is: {3}\nManufacturer name: {4}\nProduct Name: {5}\nSCB Number: 0x{6:X}\nDevice Type: {7}\nDevice Class: {8}\n\n",
                            cyDeviceInfoList[index].numInterfaces,
                            cyDeviceInfoList[index].vidPid.vid,
                            cyDeviceInfoList[index].vidPid.pid,
                            cyDeviceInfoList[index].serialNum,
                            cyDeviceInfoList[index].manufacturerName,
                            cyDeviceInfoList[index].productName,
                            cyDeviceInfoList[index].deviceBlock,
                            cyDeviceInfoList[index].deviceType[0],
                            cyDeviceInfoList[index].deviceClass[0]);
                    }

                    deviceIndexAtSCB0 = index;
                    deviceArray[arrindex] = (uint)index;
                    arrindex++;
                }
            }

            if (CommonProperty.IsDebugMode) Console.WriteLine("\nConnected Device index: {0}, {1}\n", deviceArray[0], deviceArray[1]);

            if (CommonProperty.IsDebugMode) Console.WriteLine("===================CyGetDeviceInfoVidPid End===================");

            if (arrindex < 2)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("The number of Engine Device ERROR");
                return;
            }

            deviceA.deviceNumber = (byte)deviceArray[0];
            deviceB.deviceNumber = (byte)deviceArray[1];

            // 핸들 할당
            cyReturnStatusA = CyOpen(deviceA.deviceNumber, 0, ref deviceA.cy_HANDLE);
            cyReturnStatusB = CyOpen(deviceB.deviceNumber, 0, ref deviceB.cy_HANDLE);

            if (cyReturnStatusA == CY_RETURN_STATUS.CY_SUCCESS && cyReturnStatusB == CY_RETURN_STATUS.CY_SUCCESS)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("Assignment of Engine A and B");

                deviceA.gpioNum = 2;
                deviceB.gpioNum = 2;

                deviceA.gpioValue = 1;
                deviceB.gpioValue = 1;

                //deviceA.cy_I2C_DATA_CONFIG.isStopBit = true;
                //deviceB.cy_I2C_DATA_CONFIG.isStopBit = true;
                deviceA.cy_I2C_DATA_CONFIG.isNakBit = true;
                deviceB.cy_I2C_DATA_CONFIG.isNakBit = true;

                // 각각의 핸들의 해당 Gpio를 제어
                cyReturnStatusA = CySetGpioValue(deviceA.cy_HANDLE, deviceA.gpioNum, deviceA.gpioValue);
                cyReturnStatusB = CySetGpioValue(deviceB.cy_HANDLE, deviceB.gpioNum, deviceB.gpioValue);

                // 각각 핸들의 I2C 통신 설정을 진행
                cyReturnStatusA = CySetI2cConfig(deviceA.cy_HANDLE, ref deviceA.cy_I2C_CONFIG);
                cyReturnStatusB = CySetI2cConfig(deviceB.cy_HANDLE, ref deviceB.cy_I2C_CONFIG);

                return;
            }

            else
                return; // 연결 실패
        }
    }
}
