using System.Collections.Generic;
using AngusChangyiMods.Core;

namespace AngusChangyiMods.Core.DefinitionProcessing.Test
{
    public class MockDefinition : DefBase
    {
        public string stringProp;
        public int intProp;
        public bool boolProp;

        public List<string> listProp;
    }
    
    public class MockExtension : DefExtension { }
    
    public class AnotherMockExtension : DefExtension { }
    
    public class MockComponent : ComponentProperty { }
    
    public class AnotherMockComponent : ComponentProperty {  }
}