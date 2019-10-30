//
// Klak - Utilities for creative coding with Unity
//
// Copyright (C) 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEngine;
using System.Reflection;
using Klak.Wiring;

namespace Fiftytwo
{
    [AddComponentMenu("Klak/Wiring/★/Shared Material Color Out")]
    public class SharedMaterialColorOut : NodeBase
    {
        #region Editable properties

        [SerializeField]
        Renderer _target = null;

        [SerializeField]
        string _propertyName = null;

        #endregion

        #region Node I/O

        [Inlet]
        public Color colorInput {
            set {
                if (!enabled || _target == null || _propertyID < 0) return;
                _target.sharedMaterial.SetColor(_propertyID, value);
            }
        }

        #endregion

        #region Private members

        int _propertyID = -1;

        void OnEnable()
        {
            if (!string.IsNullOrEmpty(_propertyName))
                _propertyID = Shader.PropertyToID(_propertyName);
        }

        #endregion
    }
}
