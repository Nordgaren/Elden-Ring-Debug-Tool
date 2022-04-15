sub rsp,0x48
mov r8,0x{0:X2}		;Item Give Struct
lea rdx,[r8+0x20]	
mov r10,0x{1:X2}	;MapItemMan
mov rcx,[r10]	
mov [rdx],0x1
call 0x{2:X2}
add rsp,0x48
ret
