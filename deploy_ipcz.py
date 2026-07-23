import ftplib
import os
import time

def deploy():
    HOST = os.environ.get('FTP_HOST', 'windows11.aspone.cz')
    USER = os.environ.get('FTP_USER', 'EkoBio.org_lordkikin')
    PASS = os.environ.get('FTP_PASS', 'YOUR_FTP_PASSWORD')
    LOCAL_ROOT = 'publish'
    REMOTE_ROOT = '/www/ipcz'

    ftp = ftplib.FTP(HOST)
    ftp.login(USER, PASS)
    
    def ensure_dir(path):
        parts = path.split('/')
        current = REMOTE_ROOT
        try:
            ftp.mkd(REMOTE_ROOT)
        except:
            pass
        ftp.cwd(REMOTE_ROOT)
        for part in parts:
            if not part: continue
            try:
                ftp.mkd(part)
            except:
                pass
            ftp.cwd(part)

    for root, dirs, files in os.walk(LOCAL_ROOT):
        rel_dir = os.path.relpath(root, LOCAL_ROOT).replace('\\', '/')
        if rel_dir == '.': rel_dir = ''
        
        for file in files:
            local_path = os.path.join(root, file)
            remote_path = (rel_dir + '/' + file) if rel_dir else file
            
            print(f"Uploading {remote_path}...")
            
            is_locked_type = file.lower().endswith(('.dll', '.exe'))
            if is_locked_type:
                try:
                    ensure_dir(rel_dir)
                    bak_name = f"{file}.{int(time.time())}.bak"
                    ftp.rename(file, bak_name)
                    print(f"  Renamed {file} to {bak_name}")
                except Exception as e:
                    pass
            
            ensure_dir(rel_dir)
            with open(local_path, 'rb') as f:
                ftp.storbinary(f"STOR {file}", f)
            
    ftp.quit()
    print("Deployment to /www/ipcz finished successfully!")

if __name__ == "__main__":
    deploy()
