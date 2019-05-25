// Copyright 2019 Robert Adams
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using org.herbal3d.BasilX.Util;

namespace org.herbal3d.BasilX.Items {
    public enum BItemState {
        UNINITIALIZED,
        LOADING,
        FAILED,
        READY,
        SHUTDOWN
    };

    public enum BItemType {
        UNKNOWN,
        DISPLAYABLE,
        INSTANCE,
        RENDERER,
        CONTROLS,
        COMM,
        SERVICE,
        TRANSPORT
    };

    // A BItem has named properties which are defined with getters and setters.
    public delegate T BGetValue<T>();
    public delegate void BSetValue<T>(T val);
    public abstract class PropertyDefnBase {
        public string name;
        public Type type;
    }
    public class PropertyDefn<T> : PropertyDefnBase {
        public BGetValue<T> getter;
        public BSetValue<T> setter;
        public PropertyDefn() {
            type = typeof(T);
        }
        public PropertyDefn(string pName) {
            name = pName;
            type = typeof(T);
        }
    }

    public class BItem {

        public string Id;
        public Dictionary<string, PropertyDefnBase> Properties;
        public string Auth;
        public string OwnerId;
        public string Layer = "layer.default";
        public BItemType ItemType = BItemType.UNKNOWN;
        public BItemState State = BItemState.UNINITIALIZED;

        public bool DeleteInProgress = false;
        public DateTime WhenDeleted;    // time this BItem was deleted (used in deleted list)

        public BItem(string pId, string pAuth) {
        }
        public BItem(string pId, string pAuth, BItemType pItemType) {
            Id = pId;
            Properties = new Dictionary<string, PropertyDefnBase>();
            Auth = pAuth;
            DefineProperties(new List<PropertyDefnBase>() {
                new PropertyDefn<BItemType>("_Type") {
                    getter = () => { return ItemType; }
                },
                new PropertyDefn<string>("_Id") {
                    getter = () => { return Id; }
                },
                new PropertyDefn<string>("_OwnerId") {
                    getter = () => { return OwnerId; }
                },
                new PropertyDefn<BItemState>("_State") {
                    getter = () => { return State; }
                },
                new PropertyDefn<string>("_Layer") {
                    getter = () => { return Layer; }
                },
            });
            BItem.AddItem(this);

            // TODO: Check if BItemsDeleted timer is running
        }

        public T GetProperty<T>(string pPropName) {
            T ret = default(T);
            lock (Properties) {
                if (Properties.TryGetValue(pPropName, out PropertyDefnBase pbase)) {
                    // TODO: if type does not match, should we do type conversion?
                    if (pbase.type == typeof(T)) {
                        PropertyDefn<T> defn = pbase as PropertyDefn<T>;
                        if (defn.getter != null) {
                            ret = defn.getter();
                        }
                    }
                }
            }
            return ret;
        }

        // Fetch multiple property values based on a filter expression
        public List<object> FetchProperties(string pFilter) {
            return new List<object>();
        }

        // Set a property value
        public void SetProperty<T>(string pPropId, T value) {
            lock (Properties) {
                if (Properties.TryGetValue(pPropId, out PropertyDefnBase pbase)) {
                    if (pbase.type == typeof(T)) {
                        PropertyDefn<T> defn = pbase as PropertyDefn<T>;
                        if (defn.setter != null) {
                            defn.setter(value);
                        }
                    }
                }
            }
        }

        public void SetProperties(Dictionary<string, object> pPropValues) {
            lock (Properties) {
                foreach (var kvp in pPropValues) {
                    if (Properties.TryGetValue(kvp.Key, out PropertyDefnBase pbase)) {
                        if (pbase.type == kvp.Value.GetType()) {
                            try {
                                // magic to get the typed definition of SetProperty<T>
                                // https://stackoverflow.com/questions/2604743/setting-generic-type-at-runtime
                                // https://stackoverflow.com/questions/4010144/convert-variable-to-type-only-known-at-run-time
                                Type genericType = typeof(PropertyDefn<>).MakeGenericType(new Type[] { kvp.Value.GetType() });
                                var defn = Convert.ChangeType(pbase, genericType);
                                var setter = genericType.GetMethod("setter");
                                setter.Invoke(defn, new object[] { kvp.Value });
                            }
                            catch {
                                // All that didn't work
                            }
                        }
                    }
                }
            }
        }

        public void DefineProperty<T>(string pProp, PropertyDefn<T> pDefn) {
            lock (Properties) {
                Properties.Add(pProp, pDefn);
            }
        }

        public void DefineProperties(List<PropertyDefnBase> pProperties) {
            lock (Properties) {
                pProperties.ForEach(prop => {
                    Properties.Add(prop.name, prop);
                });
            }
        }

        // Set the state of the BItem.
        // Someday add actions around state changes.
        public void SetState(BItemState pNewState) {
            State = pNewState;
        }

        // Helper functions so caller doesn't need to have BItem imports for state names.
        public void SetReady() { this.SetState(BItemState.READY); }
        public void SetFailed() { this.SetState(BItemState.FAILED); }
        public void SetLoading() { this.SetState(BItemState.LOADING); }
        public void SetShutdown() { this.SetState(BItemState.SHUTDOWN); }

        // Return when this BItem is ready.
        // Caller should 'await' on this function.
        // If the BItem does not come ready in specified period, throws BasilXException.
        // Timeout is in milliseconds.
        public Task WhenReady(int pTimeoutMS) {
            return Task.Run(async () => {
                if (State != BItemState.READY) {
                    DateTime waitStart = DateTime.UtcNow;
                    while (true) {
                        if (State == BItemState.READY) {
                            break;
                        }
                        if (DeleteInProgress
                                || State == BItemState.FAILED
                                || State == BItemState.SHUTDOWN ) {
                            throw new BasilXException("Waiting for BItem that is deleted for failed");
                        }
                        if ((DateTime.UtcNow - waitStart).TotalMilliseconds > pTimeoutMS) {
                            throw new BasilXException("BItem READY timeout");
                        }
                        // Wait for a while and check for READY again
                        await Task.Delay(BasilXContext.Instance.parms.P<int>(Params.PAssetFetchCheckIntervalMS));
                    }
                }
                return;
            });
        }

        // Wait until BItem is ready. Timeout is parameterized default (probably 5 seconds)
        public Task WhenReady() {
            return WhenReady(BasilXContext.Instance.parms.P<int>(Params.PAssetFetchTimeoutMS));
        }

        // ====================================================================
        // BItem database manipulation.
        static Dictionary<string, BItem> BItems = new Dictionary<string, BItem>();
        // When BItems are deleted, they are placed in the 'ItemsDeleted'
        //    list. This list is scanned and items are removed when they
        //    are old and/or their underlying assets have settled
        static List<BItem> BItemsDeleted = new List<BItem>();

        public static void AddItem(BItem pItem) {
            lock (BItems) {
                BItems.Add(pItem.Id, pItem);
            }
        }
        public static BItem GetItem(string pId) {
            BItem ret = null;
            lock (BItems) {
                BItems.TryGetValue(pId, out ret);
            }
            return ret;
        }

        // 
        // Remove a BItem from active use by moving it from the BItems list
        //    and adding it to the deleted item list.  The latter is scanned
        //    and items removed when cleaned up.
        public static void ForgetItem(string pId) {
            lock (BItems) {
                if (BItems.TryGetValue(pId, out BItem toRemove)) {
                    BItems.Remove(pId);
                    lock (BItemsDeleted) {
                        toRemove.DeleteInProgress = true;
                        toRemove.WhenDeleted = DateTime.UtcNow;
                        BItemsDeleted.Add(toRemove);
                    }
                }
            }
        }

        // A slow but complete operation on all the BItems.
        public static void ForEachItem(Action<BItem> pOp) {
            lock (BItems) {
                foreach (var item in BItems.Values) {
                    pOp(item);
                }
            }
        }
    }
}
