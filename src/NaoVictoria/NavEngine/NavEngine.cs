﻿using NaoVictoria.Models;
using NaoVictoria.NavEngine.Utils;
using NaoVictoria.Devices.Interfaces;
using System.Collections.Generic;
using System;

namespace NaoVictoria.NavEngine
{
    public class NavEngine : INavEngine
    {
        ICurrentOrientationSensor _currentOrientationSensor;
        ICurrentPositionSensor _currentPositionSensor;
        ICurrentWindDirectionSensor _currentWindDirectionSensor;
        ICollisionSensor _collisionSensor;
        IMainSailControl _mainSailControl;
        IJibSailControl _jibSailControl;
        IRudderControl _rudderControl;

        IEnumerable<GeoPoint> _worldOceanMap;
        IEnumerable<GeoPoint> _globalPlan;

        public NavEngine(
            ICurrentOrientationSensor currentOrientationSensor,
            ICurrentPositionSensor currentPositionSensor, 
            ICurrentWindDirectionSensor currentWindDirectionSensor,
            ICollisionSensor collisionSensor,
            IRudderControl rudderControl,
            IMainSailControl mainSailControl,
            IJibSailControl jibSailControl,
            IEnumerable<GeoPoint> worldOceanMap,
            IEnumerable<GeoPoint> globalPlan)
        {
            _currentOrientationSensor = currentOrientationSensor;
            _currentPositionSensor = currentPositionSensor;
            _currentWindDirectionSensor = currentWindDirectionSensor;
            _collisionSensor = collisionSensor;
            _rudderControl = rudderControl;
            _mainSailControl = mainSailControl;
            _jibSailControl = jibSailControl;
            _worldOceanMap = worldOceanMap;
            _globalPlan = globalPlan;
        }

        public void Navigate()
        {
            RoutePlanner routePlanner = new RoutePlanner(_currentPositionSensor, _globalPlan);
            LandCollisionAvoidance landCollisionAvoidance = new LandCollisionAvoidance(_worldOceanMap);
            CollisionAvoidanceDirection collisionAvoidance = new CollisionAvoidanceDirection(_currentOrientationSensor, _collisionSensor);
            SailingDirection sailingDirection = new SailingDirection(_currentOrientationSensor, _currentWindDirectionSensor);
            Locomotion locomotion = new Locomotion(_currentOrientationSensor, _currentWindDirectionSensor);
            double directionRadian;

            var nextRouteCheckPoint = routePlanner.GetNextClosestCheckpoint();

            directionRadian = _currentPositionSensor.GetReading().BearingTo(nextRouteCheckPoint);

            if (landCollisionAvoidance.IsInCollisionAvoidance())
            {
                directionRadian = landCollisionAvoidance.GetDirectionInRadians(nextRouteCheckPoint);
            }

            if (collisionAvoidance.IsInCollisionAvoidance())
            {
                directionRadian = collisionAvoidance.GetDirectionInRadians();
            }

            directionRadian = sailingDirection.GetDirectionInRadians(directionRadian);

            locomotion.RotateTo(directionRadian);

            if (_collisionSensor.GetDistanceToObject() > 50)
            {
                _rudderControl.MoveTo(0);
                _mainSailControl.MoveTo(0.5 * Math.PI);
                _jibSailControl.MoveTo(0);

            } else
            {
                _rudderControl.MoveTo(Math.PI);
                _mainSailControl.MoveTo(0);
                _jibSailControl.MoveTo(0.5 * Math.PI);
            }
        }
    }
}