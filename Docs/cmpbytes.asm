# Compare 1 to 4 bytes from two pointers

Argumentos : a0, a1, a2

cmpbytes :

  beq  a2,zero,pos_088E6E74  # El argumento 2 no es 0
  li  v0,0                   # A0 y A1 están alineados a 4
  or  v0,a0,a1               #
  andi  v0,v0,0x3            #
  bne  v0,zero,pos_088E6E48  # Si no lo están
    + move  v0,a2            # ??
  sltiu  v0,a2,0x4           # v0 = ($a2 < 0x4)
  move  a3,a0                # a3 = $a0
  bne  v0,zero,pos_088E6E3C  # Si $a2 < 0x4
    + move  t0,a1            # t0 = $a1
  lw  v1,0x0(a0)             # v1 = [$a0] (32 bits)
  lw  v0,0x0(a1)             # v0 = [$a1] (32 bits)
  beq  v1,v0,pos_088E6EC8    # Si [$a0] == [$a1]
    + lui  t1,0xFEFE         # t1 = 0xFEFE

pos_088E6E3C :

  move  a0,a3                #

pos_088E6E40 :

  move  a1,t0                #
  move  v0,a2                #

pos_088E6E48 :

  beq  v0,zero,pos_088E6EBC  # Si a2 == 0
    + addiu  a2,a2,-0x1      # a2--
  lbu  a3,0x0(a0)            # a3 = [a0] (8 bits)
  lbu  t0,0x0(a1)            # t0 = [a1] (8 bits)
  seb  v1,a3                 # Extiende el signo
  seb  v0,t0                 # Extiende el signo
  beq  v1,v0,pos_088E6E7C    # Si son iguales
    + move  t1,a3            #

pos_088E6E68 :

  andi  v1,a3,0xFF

pos_088E6E6C :

  andi  v0,t0,0xFF
  subu  v0,v1,v0

pos_088E6E74 :

  jr  ra
  nop

pos_088E6E7C :

  addiu  a0,a0,0x1
  addiu  a1,a1,0x1
  beq  a2,zero,pos_088E6EB4
  seb  v0,t1
  beq  v0,zero,pos_088E6EB4
  addiu  a2,a2,-0x1
  lbu  a3,0x0(a0)
  lbu  t0,0x0(a1)
  seb  v1,a3
  seb  v0,t0
  beq  v1,v0,pos_088E6E7C
  move  t1,a3
  j  pos_088E6E6C
  andi  v1,a3,0xFF

pos_088E6EB4 :

  jr  ra
  li  v0,0

pos_088E6EBC :

  lbu  a3,0x0(a0)
  j  pos_088E6E68
  lbu  t0,0x0(a1)

pos_088E6EC8 :

  lui  a1,0x8080
  ori  t1,t1,0xFEFF
  ori  a1,a1,0x8080
  addiu  a2,a2,-0x4

pos_088E6ED8 :

  beq  a2,zero,pos_088E6EB4
  sltiu  a0,a2,0x4
  lw  v0,0x0(a3)
  addiu  t0,t0,0x4
  nor  v1,zero,v0
  addu  v0,v0,t1
  and  v0,v0,v1
  and  v0,v0,a1
  bne  v0,zero,pos_088E6EB4
  addiu  a3,a3,0x4
  bnel  a0,zero,pos_088E6E40
  move  a0,a3
  lw  v1,0x0(a3)
  lw  v0,0x0(t0)
  beql  v1,v0,pos_088E6ED8
  addiu  a2,a2,-0x4
  j  pos_088E6E40
  move  a0,a3
