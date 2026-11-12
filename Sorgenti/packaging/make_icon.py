"""Generate the original Anatomiva joint mark (authoring only; icon is checked in)."""
from PIL import Image,ImageDraw
import struct
from pathlib import Path
im=Image.new('RGBA',(256,256),(25,48,61,255));d=ImageDraw.Draw(im)
d.line((72,49,72,177),fill='#e1f4ee',width=26)
d.line((72,177,198,113),fill='#55c5b1',width=26)
d.ellipse((55,160,89,194),fill='#f4bf55')
d.ellipse((59,36,85,62),fill='#e1f4ee')
d.ellipse((185,100,211,126),fill='#55c5b1')
d.arc((18,123,126,231),start=270,end=333,fill='#f4bf55',width=5)
entries=[];data=[];offset=6+16*4
for n in [16,32,48,64]:
 pic=im.resize((n,n),Image.Resampling.LANCZOS)
 raw=b''.join(pic.crop((0,y,n,y+1)).tobytes('raw','BGRA') for y in reversed(range(n)));mask=bytes(((n+31)//32)*4*n)
 blob=struct.pack('<IiiHHIIiiII',40,n,n*2,1,32,0,len(raw)+len(mask),0,0,0,0)+raw+mask
 entries.append(struct.pack('<BBBBHHII',n,n,0,0,1,32,len(blob),offset));data.append(blob);offset+=len(blob)
Path(__file__).with_name('app.ico').write_bytes(struct.pack('<HHH',0,1,4)+b''.join(entries)+b''.join(data))
