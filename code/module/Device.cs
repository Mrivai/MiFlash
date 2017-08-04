
using System;
using XiaoMiFlash.code.bl;

namespace XiaoMiFlash.code.module
{
    public class Device
    {
        private string _status = "";
        private string _result = "";
        private int _id;
        private string _name;
        private float _progress;
        private DateTime _startTime;
        private float _elapse;
        private bool? _isdone;
        private bool _isupdate;
        private DeviceCtrl _devicectrl;

        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public float Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
            }
        }

        public float Elapse
        {
            get
            {
                return _elapse;
            }
            set
            {
                _elapse = value;
            }
        }

        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public string Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
            }
        }

        public bool? IsDone
        {
            get
            {
                return _isdone;
            }
            set
            {
                _isdone = value;
            }
        }

        public bool IsUpdate
        {
            get
            {
                return _isupdate;
            }
            set
            {
                _isupdate = value;
            }
        }

        public DeviceCtrl DeviceCtrl
        {
            get
            {
                return _devicectrl;
            }
            set
            {
                _devicectrl = value;
            }
        }
    }
}
