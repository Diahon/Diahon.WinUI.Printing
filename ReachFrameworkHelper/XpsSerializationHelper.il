.class public abstract auto ansi sealed beforefieldinit System.Windows.Xps.Serialization.XpsSerializationHelper extends [System.Runtime]System.Object
{
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
        01 00 01 00 00
    )
    .method public hidebysig static void WriteToXpsDocumentPackageTarget(class [System.Runtime]System.Object target, class [PresentationCore]System.Windows.Documents.DocumentPaginator paginator, class [ReachFramework]System.Windows.Xps.Serialization.XpsSerializationPrintTicketRequiredEventHandler printTicketCallback, class [ReachFramework]System.Windows.Xps.Serialization.XpsSerializationProgressChangedEventHandler progressCallback) cil managed
    {
        .locals (
            [0] class [ReachFramework]System.Windows.Xps.Packaging.XpsOMPackagingPolicy policy,
            [1] class [ReachFramework]System.Windows.Xps.Serialization.RCW.IXpsOMPackageWriter packageWriter,
            [2] class [ReachFramework]System.Windows.Xps.Serialization.XpsOMSerializationManager manager
        )

        ldarg target
        castclass class [ReachFramework]System.Windows.Xps.Serialization.RCW.IXpsDocumentPackageTarget
        newobj instance void [ReachFramework]System.Windows.Xps.Packaging.XpsOMPackagingPolicy::.ctor(class [ReachFramework]System.Windows.Xps.Serialization.RCW.IXpsDocumentPackageTarget)
        stloc policy

        ldloc policy
        call instance void [ReachFramework]System.Windows.Xps.Packaging.XpsOMPackagingPolicy::EnsureXpsOMPackageWriter()

        ldloc policy
        ldfld class [ReachFramework]System.Windows.Xps.Serialization.RCW.IXpsOMPackageWriter [ReachFramework]System.Windows.Xps.Packaging.XpsOMPackagingPolicy::_currentFixedDocumentSequenceWriter
        stloc packageWriter

        ldloc policy
        ldc.i4.0 // false
        newobj instance void [ReachFramework]System.Windows.Xps.Serialization.XpsOMSerializationManager::.ctor(class [ReachFramework]System.Windows.Xps.Packaging.XpsOMPackagingPolicy, bool)
        stloc manager

        ldloc manager
        ldarg printTicketCallback
        call instance void [ReachFramework]System.Windows.Xps.Serialization.XpsOMSerializationManager::add_XpsSerializationPrintTicketRequired(class [ReachFramework]System.Windows.Xps.Serialization.XpsSerializationPrintTicketRequiredEventHandler)

        ldloc manager
        ldarg progressCallback
        call instance void [ReachFramework]System.Windows.Xps.Serialization.XpsOMSerializationManager::add_XpsSerializationProgressChanged(class [ReachFramework]System.Windows.Xps.Serialization.XpsSerializationProgressChangedEventHandler)

        ldloc manager
        ldarg paginator
        call instance void [ReachFramework]System.Windows.Xps.Serialization.XpsOMSerializationManager::SaveAsXaml(class [System.Runtime]System.Object)

        ldloc manager
        call instance void [ReachFramework]System.Windows.Xps.Serialization.XpsOMSerializationManager::Commit()

        // Success: https://github.com/dotnet/wpf/blob/906eb2878bbe4d5f86e77e9a0d4f85815e9f5aaf/src/Microsoft.DotNet.Wpf/src/System.Printing/CPP/src/XpsCompatiblePrinter.cpp#L125
        // Error: https://github.com/dotnet/wpf/blob/906eb2878bbe4d5f86e77e9a0d4f85815e9f5aaf/src/Microsoft.DotNet.Wpf/src/System.Printing/CPP/src/XpsCompatiblePrinter.cpp#L162
        ldloc packageWriter
        callvirt instance void [ReachFramework]System.Windows.Xps.Serialization.RCW.IXpsOMPackageWriter::Close();

        ret
    }
}