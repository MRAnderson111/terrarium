import re
path = r"D:\Unity\terrarium\Terrarium\Assets\YoYoTest\TerrariumLevel 1.unity"
text = open(path, encoding="utf-8").read()
docs = re.split(r"\n--- !u!", text)
gos = {}
transforms = {}
for d in docs:
    m = re.search(r"^(\d+) &(\d+)\n", d)
    if not m:
        continue
    typ, fid = m.group(1), m.group(2)
    if typ == "1":
        name = re.search(r"m_Name: (.*)", d)
        comps = re.findall(r"- component: \{fileID: (\d+)\}", d)
        active = re.search(r"m_IsActive: (\d)", d)
        gos[fid] = {
            "name": name.group(1).strip() if name else "?",
            "comps": comps,
            "active": active.group(1) if active else "?",
        }
    if typ in ("4", "224"):
        go = re.search(r"m_GameObject: \{fileID: (\d+)\}", d)
        father = re.search(r"m_Father: \{fileID: (\d+)\}", d)
        chunk = d.split("m_Children:")[-1].split("m_Father:")[0] if "m_Children:" in d else ""
        children = re.findall(r"- \{fileID: (\d+)\}", chunk)
        transforms[fid] = {
            "go": go.group(1) if go else None,
            "father": father.group(1) if father else "0",
            "children": children,
        }

def dump(tid, indent=0, limit=5):
    if indent > limit:
        return
    t = transforms.get(tid)
    if not t:
        print("  " * indent + "[missing %s]" % tid)
        return
    g = gos.get(t["go"], {})
    print("  " * indent + "%s active=%s tf=%s" % (g.get("name", "?"), g.get("active", "?"), tid))
    for c in t["children"]:
        dump(c, indent + 1, limit)

print("=== ROOT CANVAS ===")
dump("2114707062", 0, 3)
print("=== PROJECT CANVAS ===")
dump("6254119161575108975", 0, 2)
print("=== ProjectProgram ===")
dump("546235433", 0, 1)
