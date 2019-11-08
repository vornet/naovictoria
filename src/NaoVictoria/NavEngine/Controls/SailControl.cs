﻿using Iot.Device.Pwm;
using Iot.Device.ServoMotor;
using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Text;

namespace NaoVictoria.NavEngine.Controls
{
    public class SailControl
    {
        Pca9685 _pca9685;
        public SailControl()
        {
            var busId = 1;
            var selectedI2cAddress = 0b000000;     // A5 A4 A3 A2 A1 A0
            var deviceAddress = Pca9685.I2cAddressBase + selectedI2cAddress;

            var settings = new I2cConnectionSettings(busId, deviceAddress);
            var device = I2cDevice.Create(settings);
            _pca9685 = new Pca9685(device)
        }

        static ServoMotor CreateServo(Pca9685 pca9685, int channel)
        {
            return new ServoMotor(
                pca9685.CreatePwmChannel(channel),
                1, 450 , 2660);
        }

        public void MoveTo(double angleRadians)
        {
            using (var servo = CreateServo(_pca9685, 0))
            {
                servo.WriteAngle(angleRadians);         
            }
        }
    }
}
