sub rsp,48
mov r8, {0:X2}          ;ItemGiveStruct
lea rdx,[r8 + 20]
mov r10, {1:X2}         ;MapItemMap
mov rcx,[r10]
mov [rdx],01
call {2:X2}             ;ItemGive Function
add rsp,48
ret
