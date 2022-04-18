mov rcx,[0x{0:X2}] ;EventFlagMan
mov r8l,01
lea rdx,[{1:X2}]	;Event Flag ID
mov [rdx],r13d

sub rsp,28
call {2:X2}			;SetEventFlagFunction
add rsp,28
ret