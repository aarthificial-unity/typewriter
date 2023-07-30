using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Typewriter {
  internal static class UidGenerator {
    private const string _machineIdPrefKey =
      "Aarthificial.Typewriter.Common.UidGenerator.MachineId";
    private const string _sequencePrefKey =
      "Aarthificial.Typewriter.Common.UidGenerator.Sequence";

    private const int _machineIdBits = 10;
    private const int _sequenceBits = 12;

    private static readonly int _maxMachineId =
      (int)(Mathf.Pow(2, _machineIdBits) - 1);
    private static readonly int _maxSequence =
      (int)(Mathf.Pow(2, _sequenceBits) - 1);

    public static int GetNextKey() {
      // If we are generating another id in the same millisecond then we need to increment the sequence
      // or wait till the next millisecond if we have exhausted our sequences.
      var id = MachineId << _sequenceBits;
      id |= Sequence++;
      return id;
    }

#if UNITY_EDITOR
    private static int _sequence = EditorPrefs.GetInt(_sequencePrefKey, 0);
#else
     private static uint _sequence = 0;
#endif

    private static int Sequence {
      get => _sequence;
      set {
        _sequence = value & _maxSequence;
#if UNITY_EDITOR
        EditorPrefs.SetInt(_sequencePrefKey, _sequence);
#endif
      }
    }

    private static int _machineId;

    private static int MachineId {
      get {
        if (_machineId == 0) {
          _machineId = GetMachineId();
        }
        return _machineId;
      }
      set {
        _machineId = Mathf.Clamp(value, 1, _maxMachineId);
#if UNITY_EDITOR
        EditorPrefs.SetInt(_machineIdPrefKey, _machineId);
#endif
      }
    }

    private static int GetMachineId() {
#if UNITY_EDITOR
      var id = EditorPrefs.GetInt(_machineIdPrefKey, 0);
      if (id != 0) {
        return id;
      }

      foreach (var nic in NetworkInterface.GetAllNetworkInterfaces()) {
        if (nic.OperationalStatus == OperationalStatus.Up) {
          var address = nic.GetPhysicalAddress().ToString();
          return address.GetHashCode() & _maxMachineId;
        }
      }
#endif
      return Random.Range(0, _maxMachineId);
    }
  }
}
