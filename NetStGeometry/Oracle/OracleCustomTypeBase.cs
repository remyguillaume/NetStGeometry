using System;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using INullable = System.Data.SqlTypes.INullable;

namespace Gry.ArcGis.NetStGeometry.Oracle
{
    /// <summary>
    /// Basis type repesenting an Oracle UDT object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class OracleCustomTypeBase<T> : INullable, IOracleCustomType, IOracleCustomTypeFactory
        where T : OracleCustomTypeBase<T>, new()
    {
        [NonSerialized]
        private OracleConnection _connection;
        private IntPtr _pUdt;

        #region INullable Members

        public virtual bool IsNull { get; private set; }

        #endregion

        #region IOracleCustomType Members

        public void FromCustomObject(OracleConnection connection, IntPtr pUdt)
        {
            _connection = connection;
            _pUdt = pUdt;

            MapFromCustomObject();
        }

        public void ToCustomObject(OracleConnection connection, IntPtr pUdt)
        {
            _connection = connection;
            _pUdt = pUdt;

            MapToCustomObject();
        }

        #endregion

        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new T();
        }

        #endregion

        protected abstract void MapFromCustomObject();

        protected abstract void MapToCustomObject();

        public void SetValue(string oracleColumnName, object value)
        {
            if (value != null)
                OracleUdt.SetValue(_connection, _pUdt, oracleColumnName, value);
        }

        public void SetValue(int oracleColumnId, object value)
        {
            if (value != null)
                OracleUdt.SetValue(_connection, _pUdt, oracleColumnId, value);
        }

        public TU GetValue<TU>(string oracleColumnName)
        {
            if (OracleUdt.IsDBNull(_connection, _pUdt, oracleColumnName))
            {
                if (default(TU) is ValueType)
                    throw new Exception("Error converting Oracle User Defined Type to .Net Type");

                return default(TU);
            }

            return (TU)OracleUdt.GetValue(_connection, _pUdt, oracleColumnName);
        }

        public TU GetValue<TU>(int oracleColumnId)
        {
            if (OracleUdt.IsDBNull(_connection, _pUdt, oracleColumnId))
            {
                if (default(TU) is ValueType)
                    throw new Exception("Error converting Oracle User Defined Type to .Net Type");

                return default(TU);
            }

            return (TU)OracleUdt.GetValue(_connection, _pUdt, oracleColumnId);
        }
    }
}