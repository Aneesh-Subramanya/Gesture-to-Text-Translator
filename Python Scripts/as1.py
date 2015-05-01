with open('cricket.txt','r') as infile,open('grammar2.xml','w') as outfile:
    outfile.write('<grammar version="1.0" xml:lang="en-US" root="rootRule" tag-format="semantics/1.0-literals" xmlns="http://www.w3.org/2001/06/grammar">\n')
    outfile.write('\t<rule id="rootRule">\n')
    outfile.write('\t\t<one-of>\n')
    for line in infile:
        words=line.split()
        word=words[0]
        outfile.write('\t\t\t<item>\n')
        outfile.write('\t\t\t\t<tag>'+str(word)+'</tag>\n')
        outfile.write('\t\t\t\t<one-of>\n')
        outfile.write('\t\t\t\t\t<item>'+str(word)+'</item>\n')
        outfile.write('\t\t\t\t</one-of>\n')
        outfile.write('\t\t\t</item>\n')
    outfile.write('\t\t</one-of>\n')
    outfile.write('\t</rule>\n')
    outfile.write('</grammar>\n')
        
        
        
        
        
