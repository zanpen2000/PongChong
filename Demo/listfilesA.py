import os
def walk_dir(dir,fileinfo,topdown=True):
    fileinfo = open('list.txt','w')
    count = 0
    for root, dirs, files in os.walk(dir, topdown):
        for name in files:
            fileinfo.write(os.path.join(root,name) + '\n')
            count = count + 1
    return count

walk_dir(dir,fileinfo)

#dir = raw_input('please input the path:')
#fileinfo = open('list.txt','w')
#count = walk_dir(dir,fileinfo)
#print('file count:' + str(count))



    
