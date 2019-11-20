﻿using Iot.Device.Pwm;
using Iot.Device.ServoMotor;
using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Text;

namespace NaoVictoria.NavEngine.Controls
{
    public class RudderControl
    {
        ServoMotor _servoMotor;

        public RudderControl()
        {
            var busId = 1;
            var selectedI2cAddress = 0b000000;     // A5 A4 A3 A2 A1 A0
            var deviceAddress = Pca9685.I2cAddressBase + selectedI2cAddress;

            var settings = new I2cConnectionSettings(busId, deviceAddress);
            var device = I2cDevice.Create(settings);
            var pca9685 = new Pca9685(device);
            _servoMotor = new ServoMotor(pca9685.CreatePwmChannel(0), Math.PI, 530, 1180);
        }

        public void MoveTo(double angleRadians)
        {
            _servoMotor.WriteAngle(angleRadians);
        }
    }
}
