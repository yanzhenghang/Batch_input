# $language = "python"
# $interface = "1.0"
import codecs
import os
import time
import codecs

#ssh主机的ip
host = '131.255.73.1'
host_sub = '131.255.73.60'
#ssh主机的用户名
user = 'a'
#主机的密码
passwd = 'a'
#enable主机的密码
enable_passwd = 'a'
#判断关键字
error_key=['not running','not exist','not configured','Unrecognized','%','Invalid','error']
correct_key=['CPU utilization for','show logging']
not_command_tag=['#','command','']
not_filePath_tag=['#','']
res_file_prefix="batchfileExecute_result"
#/////////////////////para over/////////////////
class myCRTSession:
	res_file_prefix="batchfileExecute_result"
	autocommand=""
	current_mod=[]
	#type="telnet" or "ssh"
	def __init__(self, host,type='telnet', user='a',passwd='a',enable_passwd='a',connectedPromteFlag=["#",">",'login'],loginPromteFlag="#",port=23):
		self.connected_flag=False
		self.loginFlag=False
		self.type = type
		self.host = host
		self.user=user
		self.passwd=passwd
		self.enable_passwd=enable_passwd
		self.port=port
		self.connectedPromteFlag=connectedPromteFlag
		self.loginPromteFlag=loginPromteFlag
	def my_session_connect(self):		
		result=0
		# def main():
		#进行cmd操作连接创建新的session连接
		try_times=1
		cmd = str(self.type)+" %s %d" % (self.host,self.port)
		while (crt.Session.Connected==0) :
			crt.Session.Connect('/'+cmd)
			result = self.myWaitStrs(self.connectedPromteFlag,10)
			crt.Sleep(2000*try_times)
			try_times=try_times+1
		self.connected_flag=True
		return True
	def my_telnet_connect(self):		
		#进行cmd操作连接创建新的session连接
		cmd = str(self.type)+" %s %d" % (self.host,self.port)
		telnet_result=False
		try_times=1
		while (telnet_result==False):
			self.mysend(cmd)
			telnet_result =self.myWaitStrs(self.connectedPromteFlag,20)
			crt.Sleep(2000*try_times)
			try_times=try_times+1
		self.connected_flag=True
		return True
	def login_proc(self):
		# crt.Dialog.MessageBox('login')
		enable_flag=0
		ilogin=1
		self.mysend(self.user)
		result = self.myWaitStrs([self.loginPromteFlag,'login','>','password','(yes/no)','#'],10)
		while(1):		
			# crt.Dialog.MessageBox(str(result))
			#当屏幕出现login:字符
			if result==0:
				# self.my_disconnect(0)
				self.mysend("")
			if result == 1:
				break
			elif result == 2:
				self.mysend(user)
			#当屏幕出现>字符
			elif result == 3:
				self.mysend("enable")
				enable_flag=1
			#当屏幕出现password:字符
			elif result == 4:
				if enable_flag:
					# crt.Dialog.MessageBox("enable passwd:"+str(enable_passwd))
					self.mysend(enable_passwd)
					enable_flag=0
				else:
					# crt.Dialog.MessageBox("passwd:"+str(passwd))
					self.mysend(passwd)
			#屏幕出现(yes/no)等相关字符
			elif result == 5:
				self.mysend('yes')
			#等待屏幕出现']#'字符
			elif result == 6:
				break
			else:
				ilogin=ilogin+1
				self.mysend("")
				crt.Sleep(2000*ilogin)
			result = self.myWaitStrs([self.loginPromteFlag,'login','>','password','(yes/no)','#'],5)
		self.loginFlag=True
		return True
	def myWaitStrs(self,waitStrs,timeout):
		# crt.Dialog.MessageBox("entry wait strings")
		pro_res=0
		try:
			pro_res=crt.Screen.WaitForStrings(waitStrs,timeout)
		except Exception as err:
			self.myconnect()	
		return pro_res
	def myWaitStr(self,waitStr,timeout):
		pro_res=False
		try:
			pro_res=crt.Screen.WaitForString(waitStr,timeout)
		except Exception as err:
			self.myconnect()	
		return pro_res
	def myReadStrs(self,waitStrs,timeout):
		para_str=""
		try:
			para_str = crt.Screen.ReadString(waitStrs,timeout)
		except Exception as err:
			self.myconnect()	
		return para_str
	def mysend(self,send_str):
		try:
			crt.Screen.Send(send_str+'\r')
		except Exception as err:
			myconnect()
		return
	def enter_to_current_mode(self):
		for each_mod in self.current_mod:
			self.mysend(each_mod)
	def myconnect(self):
		self.my_session_connect()	
		self.login_proc()
		self.mysend(self.autocommand+'\r')
		self.enter_to_current_mode()
	def my_disconnect(self,type=1):
		if type==0:
			crt.Session.Disconnect()
		else:
			self.mysend('end')
			self.myWaitStrs(['>','#'],5)
			self.mysend('logout')
			crt.Sleep(3000)
			if crt.Session.Connected==1:
				crt.Screen.WaitForStrings(['>','#'],5)
		self.connected_flag=False
		return	
#''''''''''''''''''''''''''''''''''''''''''''''''''''''

#///////////////////////////////////////////////
def is_not_process_line(not_command_tag,cmd):
	if len(cmd)<=0:
		return True
	for tag in not_command_tag:
		taglen=len(tag)
		if taglen<=len(cmd) and taglen>0:
			# print(cmd[0:taglen])
			if cmd[0:taglen]==tag:
				return True
	return False
		
def incorrect_key(error_key,correct_key,source_string):
	for correct_item in correct_key:
		if source_string.find(correct_item) >=0:
			return False
	search_res=0
	for error_item in error_key:
		if source_string.find(error_item) >=0:
			return True
def myFileOpen(cmd_path,type,method):
	h_file=""
	try:
		h_file=codecs.open(cmd_path,type,method)
	except Exception as err:	
		crt.Dialog.MessageBox('can not file the file:'+str(err))
	return h_file
def openResultFile(filePath):
	os.system('notepad '+filePath)
####################################################################################		


####################################################################################
def main():
	global error_key,correct_key,not_command_tag,not_filePath_tag
	global host,host_sub,user,passwd,enable_passwd,res_file_prefix
	proc_mysession=myCRTSession(host)
	crt.Screen.Synchronous = 1
	crt.Screen.IgnoreEscape = 1
	filePath =  crt.Dialog.FileOpenDialog("please open a command file list","open","","(*.txt)|*.txt")
	if filePath!="":
		str_res=""
		para_str=""
		res_file=""
		file_id=1
		while (file_id<1000):
			res_file=res_file_prefix+str(file_id)+".txt"
			file_id=file_id+1
			if os.path.exists(res_file)==False:
				break
		res_fa=open(res_file,"w+")
		# my_session_connect(0,host,">")	
		with open(filePath,"r") as f:
			for line in f.readlines():
				cmdfilepath=line.lstrip()
				if is_not_process_line(not_filePath_tag,cmdfilepath):
					continue
				cmd_path=line
				cmd_path=cmd_path.strip('\r')
				cmd_path=cmd_path.strip('\n') 
				res_fa.write("will open and process file:"+cmd_path+"\r\n")
				cmd_f=myFileOpen(cmd_path,"r",'utf-8-sig')
				proc_mysession.myconnect()	
				for cline in cmd_f.readlines():
					cmd=cline.lstrip()
					if is_not_process_line(not_command_tag,cmd):
						continue
					proc_mysession.mysend(cmd)
					i=5
					while(i):
						i=i-1
						para_str = proc_mysession.myReadStrs("#",20)
						if para_str.find(cmd)>=0:
							if incorrect_key(error_key,correct_key,para_str):
								res_fa.write(cmd_path+":\r\n"+para_str+"\r\n")
								res_fa.flush()
							break
				cmd_f.close()
				proc_mysession.my_disconnect(1)
				crt.Sleep(5000)
		f.close()
		proc_mysession.my_disconnect(0)
		res_fa.write(str_res)
		res_fa.flush()
		res_fa.close()
	crt.Screen.Synchronous = 0
	crt.Screen.IgnoreEscape = 0		
	os.system('notepad '+res_file)
#////////////////////////start//////////////////////////////////////
main()
