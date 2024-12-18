using Carima.File;
using System.Runtime.InteropServices;
using static Carima.Device.Device.CyUSBSerial.CyUSBSerial;

namespace Carima.Device.Device.CyUSBSerial
{
    public class CyUSBSerialControl
    {
        public const int VID = 0x04B4;
        public const int PID = 0x000A;

        static List<KeyValuePair<uint, bool>> EngineDeviceArray = new List<KeyValuePair<uint, bool>>();

        public struct SlicingByte
        {
            private uint _value;

            // uint에 접근
            public uint Value
            {
                get => _value;
                set => _value = value;
            }

            // byte 배열로 접근
            public byte[] Bytes
            {
                get
                {
                    // uint 값을 byte 배열로 변환
                    return BitConverter.GetBytes(_value);
                }
                set
                {
                    if (value.Length != 4)
                        throw new ArgumentException("Byte array must have exactly 4 elements.");
                    // byte 배열 값을 uint로 변환
                    _value = BitConverter.ToUInt32(value, 0);
                }
            }

            // 생성자
            public SlicingByte(uint value)
            {
                _value = value;
            }

            public SlicingByte(byte[] bytes)
            {
                if (bytes.Length != 4)
                    throw new ArgumentException("Byte array must have exactly 4 elements.");
                _value = BitConverter.ToUInt32(bytes, 0);
            }
        }

        SlicingByte slicingByte;

        public IntPtr cy_HANDLE;
        
        public CY_RETURN_STATUS cy_RETURN_STATUS;

        public CY_DEVICE_INFO cy_DEVICE_INFO;
        public CY_DEVICE_INFO[] cy_DEVICE_INFO_LIST = new CY_DEVICE_INFO[16];

        public CY_VID_PID cy_VID_PID;

        public CY_I2C_CONFIG cy_I2C_CONFIG;

        public CY_I2C_DATA_CONFIG cy_I2C_DATA_CONFIG;

        public CY_DATA_BUFFER cy_DATA_BUFFER_Engine_Read;
        public CY_DATA_BUFFER cy_DATA_BUFFER_Engine_Write;

        public byte cy_Number_of_Devices;

        public byte[] deviceID = new byte[16];
        public byte[] Engine_WriteBuffer = new byte[7];
        public byte[] Engine_ReadBuffer = new byte[7];

        public byte deviceNumber;

        public byte gpioNum;
        public byte gpioValue;

        public int Engine_Count;
        public int EnableByte;

        public string outputString = "";

        public void ProjectorPowerOn()
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.ProjectorPowerOn(): called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.ProjectorPowerOn(): Device Number is not assigned");
                return;
            }

            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);

            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {
                this.gpioNum = 0;
                this.gpioValue = 1;

                CySetGpioValue(this.cy_HANDLE, this.gpioNum, this.gpioValue);

                this.gpioNum = 15;
                while (true)
                {
                    CyGetGpioValue(this.cy_HANDLE, this.gpioNum, out this.gpioValue);
                    if (this.gpioValue == 1)
                        break;
                }

                this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                if (CommonProperty.IsDebugMode)
                {
                    if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                        Console.WriteLine("CyUSBSerialControl.ProjectorPowerOn(): success");
                    else
                        Console.WriteLine("CyUSBSerialControl.ProjectorPowerOn(): failed");
                }
            }
            else
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyOpen open function call ERROR: {0}", this.cy_RETURN_STATUS);
            }
        }

        public void ProjectorPowerOff()
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.ProjectorPowerOff(): called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.ProjectorPowerOff(): Device Number is not assigned");
                return;
            }

            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);

            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {
                this.gpioNum = 0;
                this.gpioValue = 0;

                CySetGpioValue(this.cy_HANDLE, this.gpioNum, this.gpioValue);

                this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                if (CommonProperty.IsDebugMode)
                {
                    if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                        Console.WriteLine("CyUSBSerialControl.ProjectorPowerOff(): success");
                    else
                        Console.WriteLine("CyUSBSerialControl.ProjectorPowerOff(): failed");
                }
            }
            else
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyOpen open function call ERROR: {0}", this.cy_RETURN_STATUS);
            }
        }

        public void LEDPowerOn()
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.LEDPowerOn(): called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.LEDPowerOn(): Device Number is not assigned");
                return;
            }

            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);

            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {
                this.gpioNum = 14;
                this.gpioValue = 1;

                CySetGpioValue(this.cy_HANDLE, this.gpioNum, this.gpioValue);

                this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                if (CommonProperty.IsDebugMode)
                {
                    if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                        Console.WriteLine("CyUSBSerialControl.LEDPowerOn(): success");
                    else
                        Console.WriteLine("CyUSBSerialControl.LEDPowerOn(): failed");
                }
            }
            else
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyOpen open function call ERROR: {0}", this.cy_RETURN_STATUS);
            }
        }

        public void LEDPowerOff()
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.LEDPowerOff(): called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.LEDPowerOff(): Device Number is not assigned");
                return;
            }

            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);

            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {
                this.gpioNum = 14;
                this.gpioValue = 0;

                CySetGpioValue(this.cy_HANDLE, this.gpioNum, this.gpioValue);

                this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                if (CommonProperty.IsDebugMode)
                {
                    if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                        Console.WriteLine("CyUSBSerialControl.LEDPowerOff(): success");
                    else
                        Console.WriteLine("CyUSBSerialControl.LEDPowerOff(): failed");
                }
            }
            else
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyOpen open function call ERROR: {0}", this.cy_RETURN_STATUS);
            }
        }

        public bool setLEDDAC(int value)
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setLEDDAC(): called");

            if (CommonProperty.IsDebugMode) Console.WriteLine("SetLEDDAC Method in : Value = {0}", value);

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setLEDDAC(): Device Number is not assigned");
                return false;
            }

            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);

            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {
                this.gpioNum = 1;
                this.cy_RETURN_STATUS = CyGetGpioValue(this.cy_HANDLE, this.gpioNum, out this.gpioValue);
                if (this.gpioValue == 0)
                {
                    if (CommonProperty.IsDebugMode) Console.WriteLine("I2C Bus is busy");
                    return false;
                }
                else
                {
                    if (CommonProperty.IsDebugMode) Console.WriteLine("I2C Bus is not busy");

                    Array.Clear(this.Engine_WriteBuffer, 0, 3);

                    this.cy_I2C_DATA_CONFIG.slaveAddress = 0x1A;
                    this.cy_I2C_DATA_CONFIG.isNakBit = true;
                    this.cy_I2C_DATA_CONFIG.isStopBit = true;

                    this.Engine_WriteBuffer[0] = (byte)0xD1;
                    this.Engine_WriteBuffer[1] = (byte)((value >> 8) & 0xFF);
                    this.Engine_WriteBuffer[2] = (byte)(value & 0xFF);

                    if (CommonProperty.IsDebugMode) Console.WriteLine("Set LED DAC Write: ");
                    if (CommonProperty.IsDebugMode) Console.WriteLine("0x{0:X}\t0x{1:X}", this.Engine_WriteBuffer[1], this.Engine_WriteBuffer[2]);

                    GCHandle bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 3;

                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (CommonProperty.IsDebugMode) Console.WriteLine("TransCount: {0}", this.cy_DATA_BUFFER_Engine_Write.transferCount);
                }

                this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                if (CommonProperty.IsDebugMode)
                {
                    if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        Console.WriteLine("CyUSBSerialControl.setLEDDAC(): success");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("CyUSBSerialControl.setLEDDAC(): failed");
                        return false;
                    }
                }
            }
            else
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyOpen open function call ERROR: {0}", this.cy_RETURN_STATUS);
                return false;
            }

            return true;
        }

        public int getLEDDAC()
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.getLEDDAC(): called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.getLEDDAC(): Device Number is not assigned");
                return -1;
            }

            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);

            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {
                this.gpioNum = 1;
                this.cy_RETURN_STATUS = CyGetGpioValue(this.cy_HANDLE, this.gpioNum, out this.gpioValue);

                if (this.gpioValue == 0)
                {
                    if (CommonProperty.IsDebugMode) Console.WriteLine("I2C Device is busy");
                    return -1;
                }
                else
                {
                    Array.Clear(this.Engine_WriteBuffer, 0, 2);
                    this.cy_I2C_DATA_CONFIG.slaveAddress = 0x1A;
                    this.cy_I2C_DATA_CONFIG.isNakBit = true;
                    this.cy_I2C_DATA_CONFIG.isStopBit = true;

                    this.Engine_WriteBuffer[0] = (byte)0x15;
                    this.Engine_WriteBuffer[1] = (byte)0xD1;

                    GCHandle bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 2;

                    if (CommonProperty.IsDebugMode) Console.WriteLine("Get LED DAC Write: ");
                    if (CommonProperty.IsDebugMode) Console.WriteLine("0x{0:X}\t0x{1:X}", this.Engine_WriteBuffer[0], this.Engine_WriteBuffer[1]);

                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (CommonProperty.IsDebugMode) Console.WriteLine("TransCount: {0}", this.cy_DATA_BUFFER_Engine_Write.transferCount);

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Write Error = {0}", this.cy_RETURN_STATUS);
                        return -1;
                    }

                    Array.Clear(this.Engine_ReadBuffer, 0, 2);

                    bufferHandle = GCHandle.Alloc(Engine_ReadBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Read.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_ReadBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Read.length = 2;

                    this.cy_RETURN_STATUS = CyI2cRead(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Read, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Read Error = {0}", this.cy_RETURN_STATUS);
                        return -1;
                    }

                    if (CommonProperty.IsDebugMode) Console.WriteLine("Get LED DAC Read: ");
                    if (CommonProperty.IsDebugMode) Console.WriteLine("0x{0:X}\t0x{1:X}", this.Engine_WriteBuffer[0], this.Engine_WriteBuffer[1]);

                    int _dac = (this.Engine_ReadBuffer[0] << 8) + this.Engine_ReadBuffer[1];

                    this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                    if (CommonProperty.IsDebugMode)
                    {
                        if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                        {
                            Console.WriteLine("CyUSBSerialControl.getLEDDAC(): success");
                            return _dac;
                        }
                        else
                        {
                            Console.WriteLine("CyUSBSerialControl.getLEDDAC(): failed");
                            return -1;
                        }
                    }
                }
            }

            return -1;
        }

        public int setFanSpeed(int value, int option)     // value: 1~100, option: DMD = 0, LED1 = 1, LED2 = 2
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setFanSpeed(): called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setFanSpeed(): Device Number is not assigned");
                return -1;
            }

            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);
            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {
                if (value > 100) value = 100;
                else if (value < 0) value = 0;

                this.gpioNum = 1;
                this.cy_RETURN_STATUS = CyGetGpioValue(this.cy_HANDLE, this.gpioNum, out this.gpioValue);

                if (this.gpioValue == 0)
                {
                    if (CommonProperty.IsDebugMode) Console.WriteLine("I2C Device is busy");
                    return -1;
                }
                else
                {
                    Array.Clear(this.Engine_WriteBuffer, 0, 2);
                    this.cy_I2C_DATA_CONFIG.slaveAddress = 0x1A;
                    this.cy_I2C_DATA_CONFIG.isNakBit = true;
                    this.cy_I2C_DATA_CONFIG.isStopBit = true;

                    this.Engine_WriteBuffer[0] = (byte)(0xEB + option);
                    this.Engine_WriteBuffer[1] = (byte)value;

                    GCHandle bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 2;

                    if (CommonProperty.IsDebugMode) Console.WriteLine("Set Fan Speed Write: ");
                    if (CommonProperty.IsDebugMode) Console.WriteLine("0x{0:X}\t0x{1:X}", this.Engine_WriteBuffer[0], this.Engine_WriteBuffer[1]);

                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (CommonProperty.IsDebugMode) Console.WriteLine("TransCount: {0}", this.cy_DATA_BUFFER_Engine_Write.transferCount);

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Write Error = {0}", this.cy_RETURN_STATUS);
                        return -1;
                    }

                    this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                    if (CommonProperty.IsDebugMode)
                    {
                        if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                        {
                            Console.WriteLine("CyUSBSerialControl.setFanSpeed(): success");
                            return 1;
                        }
                        else
                        {
                            Console.WriteLine("CyUSBSerialControl.setFanSpeed(): failed");
                            return -1;
                        }
                    }
                }
            }

            return -1;
        }

        public int setMotorControl(bool isFront, bool isDown, int stepSize)
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setMotorControl(): called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setMotorControl(): Device Number is not assigned");
                return -1;
            }

            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);
            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {
                this.gpioNum = 1;
                this.cy_RETURN_STATUS = CyGetGpioValue(this.cy_HANDLE, this.gpioNum, out this.gpioValue);

                if (this.gpioValue == 0)
                {
                    if (CommonProperty.IsDebugMode) Console.WriteLine("I2C Device is busy");
                    return -1;
                }
                else
                {
                    Array.Clear(this.Engine_WriteBuffer, 0, 2);

                    this.cy_I2C_DATA_CONFIG.slaveAddress = 0x1A;
                    this.cy_I2C_DATA_CONFIG.isNakBit = true;
                    this.cy_I2C_DATA_CONFIG.isStopBit = true;

                    this.Engine_WriteBuffer[0] = (byte)(isFront ? 0xB5 : 0xBA);
                    this.Engine_WriteBuffer[1] = (byte)((isDown == true) ? 1 : 0);

                    GCHandle bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 2;

                    if (CommonProperty.IsDebugMode) Console.WriteLine("Set Motor Control Write: ");
                    if (CommonProperty.IsDebugMode) Console.WriteLine("0x{0:X}\t0x{1:X}", this.Engine_WriteBuffer[0], this.Engine_WriteBuffer[1]);

                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Write Error = {0}", this.cy_RETURN_STATUS);
                        return -1;
                    }

                    Array.Clear(this.Engine_WriteBuffer, 0, 5);

                    this.Engine_WriteBuffer[0] = (byte)(isFront ? 0xB6 : 0xBB);
                    slicingByte.Value = (uint)stepSize;
                    this.Engine_WriteBuffer[1] = (byte)(stepSize & 0xFF);
                    this.Engine_WriteBuffer[2] = (byte)((stepSize >> 8) & 0xFF);
                    this.Engine_WriteBuffer[3] = (byte)0x32;
                    this.Engine_WriteBuffer[4] = (byte)0x00;

                    if (CommonProperty.IsDebugMode) Console.WriteLine("Set Motor Control Write: ");
                    if (CommonProperty.IsDebugMode) Console.WriteLine("0x{0:X}\t0x{1:X}", this.Engine_WriteBuffer[0], this.Engine_WriteBuffer[1]);

                    bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 5;

                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Write Error = {0}", this.cy_RETURN_STATUS);
                        return -1;
                    }

                    this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                    if (CommonProperty.IsDebugMode)
                    {
                        if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                        {
                            Console.WriteLine("CyUSBSerialControl.setMotorControl(): success");
                            return 1;
                        }
                        else
                        {
                            Console.WriteLine("CyUSBSerialControl.setMotorControl(): failed");
                            return -1;
                        }
                    }
                }
            }

            return -1;
        }

        public double getTemperatureSensor(int option)      // DMD = 0, LED = 1, LED Driver Board = 2
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.getTemperatureSensor(): called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.getTemperatureSensor(): Device Number is not assigned");
                return -1;
            }

            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);
            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {
                this.gpioNum = 1;
                this.cy_RETURN_STATUS = CyGetGpioValue(this.cy_HANDLE, this.gpioNum, out this.gpioValue);

                if (this.gpioValue == 0)
                {
                    if (CommonProperty.IsDebugMode) Console.WriteLine("I2C Device is busy");
                    return -1;
                }
                else
                {
                    byte[] writeOptionData = { 0x9C, 0x9F, 0x9E };
                    int[] readOptionDataSize = { 1, 4, 2 };
                    this.cy_I2C_DATA_CONFIG.slaveAddress = 0x1A;
                    this.cy_I2C_DATA_CONFIG.isNakBit = true;
                    this.cy_I2C_DATA_CONFIG.isStopBit = true;

                    Array.Clear(this.Engine_WriteBuffer, 0, 2);

                    this.Engine_WriteBuffer[0] = (byte)0x15;
                    this.Engine_WriteBuffer[1] = (byte)writeOptionData[option];

                    GCHandle bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = (uint)2;

                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode)
                        {
                            Console.WriteLine($"Write Error: {this.cy_RETURN_STATUS}");
                            Console.WriteLine($"Slave Address: 0x{this.cy_I2C_DATA_CONFIG.slaveAddress:X}");
                            Console.WriteLine($"Buffer Length: {this.cy_DATA_BUFFER_Engine_Write.length}");
                            Console.WriteLine($"Buffer Content: {string.Join(" ", Engine_WriteBuffer.Select(b => $"0x{b:X2}"))}");
                        }

                        return -1;
                    }

                    Array.Clear(this.Engine_ReadBuffer, 0, readOptionDataSize[option]);

                    bufferHandle = GCHandle.Alloc(Engine_ReadBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Read.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_ReadBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Read.length = (uint)readOptionDataSize[option];

                    this.cy_RETURN_STATUS = CyI2cRead(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Read, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode)
                        {
                            Console.WriteLine($"Read Error: {this.cy_RETURN_STATUS}");
                            Console.WriteLine($"Slave Address: 0x{this.cy_I2C_DATA_CONFIG.slaveAddress:X}");
                            Console.WriteLine($"Buffer Length: {this.cy_DATA_BUFFER_Engine_Read.length}");
                            Console.WriteLine($"Buffer Content: {string.Join(" ", Engine_ReadBuffer.Select(b => $"0x{b:X2}"))}");
                        }

                        return -1;
                    }

                    this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);
                    if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.getTemperatureSensor(): success");

                        if (option == 0)
                        {
                            return (double)(this.reverseBYTE(this.Engine_ReadBuffer[0]));
                        }
                        else if (option == 1)
                        {
                            return (double)((this.Engine_ReadBuffer[3] << 8) + this.Engine_ReadBuffer[2] + (float)((((this.Engine_ReadBuffer[1] << 8) + this.Engine_ReadBuffer[0])) / 65536));
                        }
                        else if (option == 2)
                        {
                            return (double)(((this.Engine_ReadBuffer[1] & 0x0F) << 4) + ((this.Engine_ReadBuffer[0] & 0xF0) >> 4) + (float)((this.Engine_ReadBuffer[0] & 0x0F) / 16));
                        }
                        else
                        {
                            if (CommonProperty.IsDebugMode) Console.WriteLine("Get Temperature Sensor Option Error");
                            return -1;
                        }
                    }
                    else
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Cy Close Return Failure....");
                        return -1;
                    }
                }
            }

            return -1;
        }

        public int getLightSensor()
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.getLightSensor(): called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.getLightSensor(): Device Number is not assigned");
                return -1;
            }

            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);
            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {
                this.gpioNum = 1;
                this.cy_RETURN_STATUS = CyGetGpioValue(this.cy_HANDLE, this.gpioNum, out this.gpioValue);

                if (this.gpioValue == 0)
                {
                    if (CommonProperty.IsDebugMode) Console.WriteLine("I2C Device is busy");
                    return -1;
                }
                else
                {
                    Array.Clear(this.Engine_WriteBuffer, 0, 2);
                    this.cy_I2C_DATA_CONFIG.slaveAddress = 0x1A;
                    this.cy_I2C_DATA_CONFIG.isNakBit = true;
                    this.cy_I2C_DATA_CONFIG.isStopBit = true;

                    this.Engine_WriteBuffer[0] = (byte)0x15;
                    this.Engine_WriteBuffer[1] = (byte)0xF7;

                    GCHandle bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 2;

                    if (CommonProperty.IsDebugMode) Console.WriteLine("Get Light Sensor Write: ");
                    if (CommonProperty.IsDebugMode) Console.WriteLine("0x{0:X}\t0x{1:X}", this.Engine_WriteBuffer[0], this.Engine_WriteBuffer[1]);

                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Write Error = {0}", this.cy_RETURN_STATUS);
                        return -1;
                    }

                    Array.Clear(this.Engine_ReadBuffer, 0, 2);

                    bufferHandle = GCHandle.Alloc(Engine_ReadBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Read.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_ReadBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Read.length = 2;

                    this.cy_RETURN_STATUS = CyI2cRead(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Read, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Read Error = {0}", this.cy_RETURN_STATUS);
                        return -1;
                    }

                    if (CommonProperty.IsDebugMode) Console.WriteLine("Get Light Sensor Read: ");
                    if (CommonProperty.IsDebugMode) Console.WriteLine("0x{0:X}\t0x{1:X}", this.Engine_ReadBuffer[0], this.Engine_ReadBuffer[1]);

                    int lightSensorValue = (this.Engine_ReadBuffer[1] << 8) + this.Engine_ReadBuffer[0];

                    this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                    if (CommonProperty.IsDebugMode)
                    {
                        if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                        {
                            Console.WriteLine("CyUSBSerialControl.getLightSensor(): success");
                            return lightSensorValue;
                        }
                        else
                        {
                            Console.WriteLine("CyUSBSerialControl.getLightSensor(): failed");
                            return -1;
                        }
                    }
                }
            }

            return -1;
        }

        public byte reverseBYTE(byte inputByte)
        {
            byte outputByte = 0;

            for (int i = 0; i < 7; i++)
            {
                outputByte |= (byte)(inputByte & 1);

                inputByte = (byte)(inputByte >> 1);
                outputByte = (byte)(outputByte << 1);
            }
            outputByte |= (byte)(inputByte & 1);

            return outputByte;
        }

        public void setProjectorsourceandTestpattern_HDMI()
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setProjectorsourceandTestpattern_HDMI(): called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setProjectorsourceandTestpattern_HDMI(): Device Number is not assigned");
                return;
            }


            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);
            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {
                this.gpioNum = 1;
                this.cy_RETURN_STATUS = CyGetGpioValue(this.cy_HANDLE, this.gpioNum, out this.gpioValue);

                if (this.gpioValue == 0)
                {
                    if (CommonProperty.IsDebugMode) Console.WriteLine("I2C Device is busy");
                    return;
                }
                else
                {
                    Array.Clear(this.Engine_WriteBuffer, 0, 2);
                    this.cy_I2C_DATA_CONFIG.slaveAddress = 0x1A;
                    this.cy_I2C_DATA_CONFIG.isNakBit = true;
                    this.cy_I2C_DATA_CONFIG.isStopBit = true;

                    this.Engine_WriteBuffer[0] = (byte)0x01;
                    this.Engine_WriteBuffer[1] = (byte)0x00;

                    GCHandle bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 2;
                    
                    if (CommonProperty.IsDebugMode) Console.WriteLine("Set HDMI Write: ");
                    if (CommonProperty.IsDebugMode) Console.WriteLine("0x{0:X}\t0x{1:X}", this.Engine_WriteBuffer[0], this.Engine_WriteBuffer[1]);

                    if (CommonProperty.IsDebugMode) Console.WriteLine("TransCount: {0}", this.cy_DATA_BUFFER_Engine_Write.transferCount);
                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Write Error = {0}", this.cy_RETURN_STATUS);
                        return;
                    }

                    this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                    if (CommonProperty.IsDebugMode)
                    {
                        if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                        {
                            Console.WriteLine("CyUSBSerialControl.setProjectorsourceandTestpattern_HDMI(): success");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("CyUSBSerialControl.setProjectorsourceandTestpattern_HDMI(): failed");
                            return;
                        }
                    }
                }
            }
        }

        public void setProjectorsourceandTestpattern_Ramp()
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setProjectorsourceandTestpattern_Ramp(): called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setProjectorsourceandTestpattern_Ramp(): Device Number is not assigned");
                return;
            }

            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);
            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {

                this.gpioNum = 1;
                this.cy_RETURN_STATUS = CyGetGpioValue(this.cy_HANDLE, this.gpioNum, out this.gpioValue);

                if (this.gpioValue == 0)
                {
                    if (CommonProperty.IsDebugMode) Console.WriteLine("I2C Device is busy");
                    return;
                }
                else
                {
                    Array.Clear(this.Engine_WriteBuffer, 0, 2);
                    this.cy_I2C_DATA_CONFIG.slaveAddress = 0x1A;
                    this.cy_I2C_DATA_CONFIG.isNakBit = true;
                    this.cy_I2C_DATA_CONFIG.isStopBit = true;

                    this.Engine_WriteBuffer[0] = (byte)0x01;
                    this.Engine_WriteBuffer[1] = (byte)0x01;

                    GCHandle bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 2;

                    if (CommonProperty.IsDebugMode) Console.WriteLine("Set Ramp Write1: ");
                    if (CommonProperty.IsDebugMode) Console.WriteLine("0x{0:X}\t0x{1:X}", this.Engine_WriteBuffer[0], this.Engine_WriteBuffer[1]);

                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (CommonProperty.IsDebugMode) Console.WriteLine("TransCount: {0}", this.cy_DATA_BUFFER_Engine_Write.transferCount);

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Write Error = {0}", this.cy_RETURN_STATUS);
                        return;
                    }

                    Array.Clear(this.Engine_WriteBuffer, 0, 2);
                    this.cy_I2C_DATA_CONFIG.isNakBit = true;
                    this.cy_I2C_DATA_CONFIG.isStopBit = true;

                    this.Engine_WriteBuffer[0] = (byte)0x11;
                    this.Engine_WriteBuffer[1] = (byte)0x01;

                    bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 2;

                    if (CommonProperty.IsDebugMode) Console.WriteLine("Set Ramp Write2: ");
                    if (CommonProperty.IsDebugMode) Console.WriteLine("0x{0:X}\t0x{1:X}", this.Engine_WriteBuffer[0], this.Engine_WriteBuffer[1]);

                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (CommonProperty.IsDebugMode) Console.WriteLine("TransCount: {0}", this.cy_DATA_BUFFER_Engine_Write.transferCount);

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Write Error = {0}", this.cy_RETURN_STATUS);
                        return;
                    }

                    this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                    if (CommonProperty.IsDebugMode)
                    {
                        if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                        {
                            Console.WriteLine("CyUSBSerialControl.setProjectorsourceandTestpattern_Ramp(): success");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("CyUSBSerialControl.setProjectorsourceandTestpattern_Ramp(): failed");
                            return;
                        }
                    }
                }
            }
        }

        public void setProjectorsourceandTestpattern_CheckerBoard()
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setProjectorsourceandTestpattern_CheckerBoard(): called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setProjectorsourceandTestpattern_CheckerBoard(): Device Number is not assigned");
                return;
            }

            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);
            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {

                this.gpioNum = 1;
                this.cy_RETURN_STATUS = CyGetGpioValue(this.cy_HANDLE, this.gpioNum, out this.gpioValue);

                if (this.gpioValue == 0)
                {
                    if (CommonProperty.IsDebugMode) Console.WriteLine("I2C Device is busy");
                    return;
                }
                else
                {
                    Array.Clear(this.Engine_WriteBuffer, 0, 2);
                    this.cy_I2C_DATA_CONFIG.slaveAddress = 0x1A;
                    this.cy_I2C_DATA_CONFIG.isNakBit = true;
                    this.cy_I2C_DATA_CONFIG.isStopBit = true;

                    this.Engine_WriteBuffer[0] = (byte)0x01;
                    this.Engine_WriteBuffer[1] = (byte)0x01;

                    GCHandle bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 2;

                    if (CommonProperty.IsDebugMode) Console.WriteLine("Set Checkerboard Write1: ");
                    if (CommonProperty.IsDebugMode) Console.WriteLine("0x{0:X}\t0x{1:X}", this.Engine_WriteBuffer[0], this.Engine_WriteBuffer[1]);

                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (CommonProperty.IsDebugMode) Console.WriteLine("TransCount: {0}", this.cy_DATA_BUFFER_Engine_Write.transferCount);

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Write Error = {0}", this.cy_RETURN_STATUS);
                        return;
                    }

                    Array.Clear(this.Engine_WriteBuffer, 0, 2);
                    this.cy_I2C_DATA_CONFIG.isNakBit = true;
                    this.cy_I2C_DATA_CONFIG.isStopBit = true;

                    this.Engine_WriteBuffer[0] = (byte)0x11;
                    this.Engine_WriteBuffer[1] = (byte)0x07;

                    bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 2;

                    if (CommonProperty.IsDebugMode) Console.WriteLine("Set Checkerboard Write2: ");
                    if (CommonProperty.IsDebugMode) Console.WriteLine("0x{0:X}\t0x{1:X}", this.Engine_WriteBuffer[0], this.Engine_WriteBuffer[1]);

                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (CommonProperty.IsDebugMode) Console.WriteLine("TransCount: {0}", this.cy_DATA_BUFFER_Engine_Write.transferCount);

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Write Error = {0}", this.cy_RETURN_STATUS);
                        return;
                    }

                    this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                    if (CommonProperty.IsDebugMode)
                    {
                        if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                        {
                            Console.WriteLine("CyUSBSerialControl.setProjectorsourceandTestpattern_CheckerBoard(): success");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("CyUSBSerialControl.setProjectorsourceandTestpattern_CheckerBoard(): failed");
                            return;
                        }
                    }
                }
            }
        }

        public void setProjectorsourceandTestpattern_SolidField()
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setProjectorsourceandTestpattern_SolidField(): called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setProjectorsourceandTestpattern_SolidField(): Device Number is not assigned");
                return;
            }

            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);
            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {
                this.gpioNum = 1;
                this.cy_RETURN_STATUS = CyGetGpioValue(this.cy_HANDLE, this.gpioNum, out this.gpioValue);

                if (this.gpioValue == 0)
                {
                    if (CommonProperty.IsDebugMode) Console.WriteLine("I2C Device is busy");
                    return;
                }
                else
                {
                    Array.Clear(this.Engine_WriteBuffer, 0, 7);
                    this.cy_I2C_DATA_CONFIG.slaveAddress = 0x1A;
                    this.cy_I2C_DATA_CONFIG.isNakBit = true;
                    this.cy_I2C_DATA_CONFIG.isStopBit = true;

                    this.Engine_WriteBuffer[0] = (byte)0x13;
                    this.Engine_WriteBuffer[1] = (byte)0xFF;
                    this.Engine_WriteBuffer[2] = (byte)0x03;
                    this.Engine_WriteBuffer[3] = (byte)0xFF;
                    this.Engine_WriteBuffer[4] = (byte)0x03;
                    this.Engine_WriteBuffer[5] = (byte)0xFF;
                    this.Engine_WriteBuffer[6] = (byte)0x03;

                    GCHandle bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 7;

                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Write Error = {0}", this.cy_RETURN_STATUS);
                        return;
                    }

                    Array.Clear(this.Engine_WriteBuffer, 0, 2);
                    this.cy_I2C_DATA_CONFIG.isNakBit = true;
                    this.cy_I2C_DATA_CONFIG.isStopBit = true;

                    this.Engine_WriteBuffer[0] = (byte)0x01;
                    this.Engine_WriteBuffer[1] = (byte)0x02;

                    bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 2;

                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Write Error = {0}", this.cy_RETURN_STATUS);
                        return;
                    }

                    this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                    if (CommonProperty.IsDebugMode)
                    {
                        if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                        {
                            Console.WriteLine("CyUSBSerialControl.setProjectorsourceandTestpattern_SolidField(): success");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("CyUSBSerialControl.setProjectorsourceandTestpattern_SolidField(): failed");
                            return;
                        }
                    }
                }
            }
        }

        public void setEngineCurrent(int engineCurrent)
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setEngineCurrent(): called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setEngineCurrent(): Device Number is not assigned");
                return;
            }

            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);
            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {

                this.gpioNum = 1;
                this.cy_RETURN_STATUS = CyGetGpioValue(this.cy_HANDLE, this.gpioNum, out this.gpioValue);

                if (this.gpioValue == 0)
                {
                    if (CommonProperty.IsDebugMode) Console.WriteLine("I2C Device is busy");
                    return;
                }
                else
                {
                    Array.Clear(this.Engine_WriteBuffer, 0, 7);
                    this.cy_I2C_DATA_CONFIG.slaveAddress = 0x1A;
                    this.cy_I2C_DATA_CONFIG.isNakBit = true;
                    this.cy_I2C_DATA_CONFIG.isStopBit = true;

                    this.Engine_WriteBuffer[0] = (byte)0x54;
                    this.Engine_WriteBuffer[1] = (byte)(engineCurrent & 0xFF);  // R
                    this.Engine_WriteBuffer[2] = (byte)((engineCurrent >> 8) & 0xFF);
                    this.Engine_WriteBuffer[3] = (byte)(engineCurrent & 0xFF);  // G
                    this.Engine_WriteBuffer[4] = (byte)((engineCurrent >> 8) & 0xFF);
                    this.Engine_WriteBuffer[5] = (byte)(engineCurrent & 0xFF);  // B
                    this.Engine_WriteBuffer[6] = (byte)((engineCurrent >> 8) & 0xFF);

                    GCHandle bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 7;

                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Write Error = {0}", this.cy_RETURN_STATUS);
                        return;
                    }

                    this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                    if (CommonProperty.IsDebugMode)
                    {
                        if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                        {
                            Console.WriteLine("CyUSBSerialControl.setEngineCurrent(): success");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("CyUSBSerialControl.setEngineCurrent(): failed");
                            return;
                        }
                    }
                }
            }
        }

        public void setEngineSeqOn()
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setEngineSeqOn(): called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setEngineSeqOn(): Device Number is not assigned");
                return;
            }

            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);
            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {
                this.gpioNum = 1;
                this.cy_RETURN_STATUS = CyGetGpioValue(this.cy_HANDLE, this.gpioNum, out this.gpioValue);

                if (this.gpioValue == 0)
                {
                    if (CommonProperty.IsDebugMode) Console.WriteLine("I2C Device is busy");
                    return;
                }
                else
                {
                    Array.Clear(this.Engine_WriteBuffer, 0, 2);

                    this.EnableByte |= 0x01 | 0x02 | 0x04;

                    this.Engine_WriteBuffer[0] = (byte)0x52;
                    this.Engine_WriteBuffer[1] = (byte)EnableByte;

                    GCHandle bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 2;
                    
                    this.cy_I2C_DATA_CONFIG.slaveAddress = (byte)0x1A;

                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Write Error = {0}", this.cy_RETURN_STATUS);
                        return;
                    }

                    this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                    if (CommonProperty.IsDebugMode)
                    {
                        if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                        {
                            Console.WriteLine("CyUSBSerialControl.setEngineSeqOn(): success");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("CyUSBSerialControl.setEngineSeqOn(): failed");
                            return;
                        }
                    }
                }
            }
        }

        public void setEngineSeqOff()
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setEngineSeqOff(): called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setEngineSeqOff(): Device Number is not assigned");
                return;
            }

            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);
            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {
                this.gpioNum = 1;
                this.cy_RETURN_STATUS = CyGetGpioValue(this.cy_HANDLE, this.gpioNum, out this.gpioValue);

                if (this.gpioValue == 0)
                {
                    if (CommonProperty.IsDebugMode) Console.WriteLine("I2C Device is busy");
                    return;
                }
                else
                {
                    Array.Clear(this.Engine_WriteBuffer, 0, 2);

                    this.EnableByte |= 0;

                    this.Engine_WriteBuffer[0] = (byte)0x52;
                    this.Engine_WriteBuffer[1] = (byte)EnableByte;

                    GCHandle bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 2;
                    
                    this.cy_I2C_DATA_CONFIG.slaveAddress = (byte)0x1A;

                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Write Error = {0}", this.cy_RETURN_STATUS);
                        return;
                    }

                    this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                    if (CommonProperty.IsDebugMode)
                    {
                        if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                        {
                            Console.WriteLine("CyUSBSerialControl.setEngineSeqOff(): success");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("CyUSBSerialControl.setEngineSeqOff(): failed");
                            return;
                        }
                    }
                }
            }
        }

        public void setEngineFlip(bool isOn, bool isFlipX)    // isFlipX = true (x축 반전; 좌우 반전) /isFlipX = false (y축 반전; 상하 반전) 
        {
            if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setEngineFlip() called");

            if (this.deviceNumber < 0)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("CyUSBSerialControl.setEngineFlip(): Device Number is not assigned");
                return;
            }
            this.cy_RETURN_STATUS = CyOpen(this.deviceNumber, 0, ref this.cy_HANDLE);
            if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
            {
                if (CommonProperty.IsDebugMode) Console.WriteLine("Set GPIO");
                this.gpioNum = 1;
                this.cy_RETURN_STATUS = CyGetGpioValue(this.cy_HANDLE, this.gpioNum, out this.gpioValue);

                if (this.gpioValue == 0)
                {
                    if (CommonProperty.IsDebugMode) Console.WriteLine("I2C Device is busy");
                    return;
                }
                else
                {
                    if (CommonProperty.IsDebugMode) Console.WriteLine("GPIO is Not BUSY");
                    Array.Clear(this.Engine_WriteBuffer, 0, 2);

                    //this.EnableByte |= 0;

                    this.cy_I2C_DATA_CONFIG.slaveAddress = 0x1A;
                    this.cy_I2C_DATA_CONFIG.isNakBit = true;
                    this.cy_I2C_DATA_CONFIG.isStopBit = true;

                    this.Engine_WriteBuffer[0] = (byte)(isFlipX ? 0x10 : 0x1F);         // Flip 축 설정 
                    this.Engine_WriteBuffer[1] = (byte)(isOn ? 0x01 : 0x00);            // Flip On Off 설정

                    GCHandle bufferHandle = GCHandle.Alloc(Engine_WriteBuffer, GCHandleType.Pinned);
                    this.cy_DATA_BUFFER_Engine_Write.buffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.Engine_WriteBuffer, 0);
                    this.cy_DATA_BUFFER_Engine_Write.length = 2;
                    
                    this.cy_RETURN_STATUS = CyI2cWrite(this.cy_HANDLE, ref this.cy_I2C_DATA_CONFIG, ref this.cy_DATA_BUFFER_Engine_Write, 5000);
                    if (bufferHandle.IsAllocated) bufferHandle.Free();

                    if (this.cy_RETURN_STATUS != CY_RETURN_STATUS.CY_SUCCESS)
                    {
                        if (CommonProperty.IsDebugMode) Console.WriteLine("Write Error = {0}", this.cy_RETURN_STATUS);
                        return;
                    }
                    
                    this.cy_RETURN_STATUS = CyClose(this.cy_HANDLE);

                    if (CommonProperty.IsDebugMode)
                    {
                        if (this.cy_RETURN_STATUS == CY_RETURN_STATUS.CY_SUCCESS)
                        {
                            Console.WriteLine("CyUSBSerialControl.setEngineFlip(): success");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("CyUSBSerialControl.setEngineFlip(): failed");
                            return;
                        }
                    }
                }
            }
        }
    }
}
