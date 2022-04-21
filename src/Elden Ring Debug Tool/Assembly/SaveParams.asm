mov rcx,0x{0:X2}		;SoloParamRepository
mov rcx,[rcx+0x{1:X2}]	;Param Offset
mov rcx,[rcx+0x80]

sub rsp,0x28
mov r14, 0x{2:X2}		;Save Param Func
call r14
add rsp,0x28
ret