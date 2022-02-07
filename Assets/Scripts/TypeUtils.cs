using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class TypeUtils 
{
    /*
     * Taken from 
     * https://coffeebraingames.wordpress.com/2017/07/16/reflection-series-part-1-from-class-name-to-instance/
     */
    public static Type GetType(string typeName) {
        // Try Type.GetType() first. This will work with types defined
        // by the Mono runtime, in the same assembly as the caller, etc.
        Type type = Type.GetType(typeName);
    
        // If it worked, then we're done here
        if(type != null) {
            return type;
        }
    
        // Attempt to search for type on the loaded assemblies
        Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach(Assembly assembly in currentAssemblies) {
            type = assembly.GetType(typeName);
            if(type != null) {
                return type;
            }
        }
    
        // If we still haven't found the proper type, we can enumerate all of the
        // loaded assemblies and see if any of them define the type
        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach(var assemblyName in referencedAssemblies) {
            // Load the referenced assembly
            var assembly = Assembly.Load(assemblyName);
            if(assembly != null) {
                // See if that assembly defines the named type
                type = assembly.GetType(typeName);
                if(type != null) {
                    return type;
                }
            }
        }
    
        // The type just couldn't be found...
        return null;
    }

    public static ConstructorInfo ResolveEmptyConstructor(Type type) {
    ConstructorInfo[] constructors = type.GetConstructors();
    foreach(ConstructorInfo constructor in constructors) {
        // we only need the default constructor
        if(constructor.GetParameters().Length == 0) {
            return constructor;
        }
    }
    
    // Can't resolve appropriate constructor. Client code should check for this.
    return null;
    }
}
