# PropertyHook
A .NET library targeting 4.6.1 that makes it easy to define an interface for another application's memory.

[Using PropertyHook](#using)  
[PHook Reference](#phook)  
[PHPointer Reference](#phpointer)  

<a name="using"></a>
# Using PropertyHook
The core of the library is the PHook class, which provides the interface to the target process through custom pointers and handles hooking, unhooking, and AOB scanning.  
To configure your own hook, first create a derived class of PHook. The selector passed to the base constructor decides which processes are eligible for hooking.  
Note: 64-bit processes can only be hooked if your application is running as 64-bit. Either specifically target x64 or target Any CPU and uncheck "Prefer 32-bit" in your project Build properties.  
```cs
public class SampleHook : PHook
{
    public SampleHook() : base(5000, 5000, p => p.ProcessName == "DarkSoulsRemastered")
    {
    
    }
}
```

Next, create pointers to the memory regions you're interested in. There are 4 different pointer classes available:
* `PHPointerBase` - a pointer to a fixed address
* `PHPointerAOBAbsolute` - a pointer that scans for an array of bytes to find its base address
* `PHPointerAOBRelative` - a pointer that scans for an AOB, then follows a relative offset found at the AOB. Used for 64-bit applications where offsets in code are often relative to the end of the instruction containing them
* `PHPointerChild` - a pointer that uses another pointer's address as its base address

AOB pointers are automatically rescanned whenever the application is hooked; they can be created with either an array of nullable bytes, where null indicates a wildcard, or a CE-style AOB string, where ? indicates a wildcard. All pointer types also accept a list of offsets; after resolving their base address the pointer will read and jump to an address at each offset in turn.
```cs
public class SampleHook : PHook
{
    private PHPointer WorldChrBase;
    private PHPointer ChrData1;
    private PHPointer ChrData2;

    public SampleHook() : base(5000, 5000, p => p.ProcessName == "DarkSoulsRemastered")
    {
        WorldChrBase = CreateBasePointer((IntPtr)0x141D151B0, 0);
        ChrData1 = CreateChildPointer(WorldChrBase, 0x68);
        ChrData2 = RegisterRelativeAOB("48 8B 05 ? ? ? ? 48 85 C0 ? ? F3 0F 58 80 AC 00 00 00", 3, 7, 0, 0x10);
    }
}
```

Finally, add whatever properties or methods you need to access specific values. Operations are safe even when no process is hooked; reads will always return 0, and writes will do nothing.
```cs
public class SampleHook : PHook
{
    // Constructor etc

    public bool DeathCam
    {
        get => WorldChrBase.ReadBoolean(0x70);
        set => WorldChrBase.WriteBoolean(0x70, value);
    }

    public int Health
    {
        get => ChrData1.ReadInt32(0x3E8);
        set => ChrData1.WriteInt32(0x3E8, value);
    }

    public int Stamina
    {
        get => ChrData1.ReadInt32(0x3F8);
        set => ChrData1.WriteInt32(0x3F8, value);
    }
    
    public Stats GetStats()
    {
        Stats stats;
        stats.Vitality = ChrData2.ReadInt32(0x40);
        stats.Attunement = ChrData2.ReadInt32(0x48);
        stats.Endurance = ChrData2.ReadInt32(0x50);
        // etc
        return stats;
    }
}

public struct Stats
{
    public int Vitality;
    public int Attunement;
    public int Endurance;
}
```

Once your hook is written, instantiate it and call the Start method to start a thread that will automatically hook and unhook any valid processes found. You can also manually call the Refresh method if you prefer.
```cs
SampleHook hook = new SampleHook();
hook.Start();

while (!Console.KeyAvailable)
{
    Console.WriteLine($"Health: {hook.Health,4}");
    if (hook.Health < 200)
        hook.Health += 200;
    Thread.Sleep(100);
}

// Not strictly necessary; the hooking thread is a background thread, so it will exit automatically
hook.Stop();
```

<a name="phook"></a>
# PHook Reference
#### Properties
* `Hooked` - whether the hook is attached to a process
* `Process` - the attached process, if any
* `Is64Bit` - whether the attached process is 32 or 64-bit
* `Handle` - the handle to the attached process, if any
* `RefreshInterval` - how often the hooking thread will check for new processes
* `MinLifetime` - minimum time a process must be running before it will be hooked
* `AOBScanSucceeded` - whether all AOBs found a match during the last scan

#### Events
* `OnHooked` - fires after attaching to a process
* `OnUnhooked` - fires after detaching from a process

#### Methods
* `Start` - starts the automatic hooking thread
* `Stop` - stops the automatic hooking thread
* `Refresh` - checks for new processes to hook
* `RescanAOB` - manually rescans all AOB pointers
* `CreateBasePointer` - creates a base address pointer
* `CreateChildPointer` - creates a child pointer
* `RegisterRelativeAOB` - creates a relative AOB pointer
* `RegisterAbsoluteAOB` - creates an absolute AOB pointer
* `UnregisterAOBPointer` - removes an AOB pointer from the automatic scanning list
* `Allocate` - allocate a memory region
* `Free` - free a memory region
* `Execute` - run a remote thread

<a name="phpointer"></a>
# PHPointer Reference
#### Methods
* `Resolve` - evaluates the base pointer and follows the offsets to find the final address

Each read and write method first resolves the address, then moves forward by the given offset before operating.
* `ReadBytes`
* `WriteBytes`
* `ReadIntPtr` - reads either a 32-bit or 64-bit address depending on the attached process
* `ReadSByte`
* `WriteSByte`
* `ReadByte`
* `WriteByte`
* `ReadBoolean`
* `WriteBoolean`
* `ReadInt16`
* `WriteInt16`
* `ReadUInt16`
* `WriteUInt16`
* `ReadInt32`
* `WriteInt32`
* `ReadUInt32`
* `WriteUInt32`
* `ReadInt64`
* `WriteInt64`
* `ReadUInt64`
* `WriteUInt64`
* `ReadSingle`
* `WriteSingle`
* `ReadDouble`
* `WriteDouble`
* `ReadString` - reads a fixed-length string with a specified encoding
* `WriteString` - writes a fixed-length string with a specified encoding
