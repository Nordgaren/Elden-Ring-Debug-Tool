mov rcx,0x{0:X2}		;EventFlagMan
mov r8b,01
lea rdx,[0x{1:X2}]		;Event Flag ID

sub rsp,0x28
call 0x{2:X2}			;SetEventFlagFunction
add rsp,0x28
ret