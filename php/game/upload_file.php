<?php
if(isset($_POST['file_size']) && isset($_POST['id_user']) && isset($_POST['folder_name']) && isset($_POST['file_name']) && $_POST['file_size'] == $_FILES['file_data']['size'])
{
	//DATA
	$id_user = $_POST['id_user'];
	$file_name = $_POST['file_name'];
	$folder_name = $_POST['folder_name'];

	$root_directory = '../../files/'.$id_user.'/'.$folder_name.'/';

	$file_path = $root_directory.$file_name;
	
	if (!file_exists($root_directory))
	{
		$oldmask = umask(0);
		mkdir($root_directory, 0775, true);
		umask($oldmask);
	}
	//files
	move_uploaded_file($_FILES['file_data']['tmp_name'], $file_path);
	chmod($file_path, 0664);
	
	echo $_POST['file_size']. ' == ' .$_FILES['file_data']['size'];
}
else
{
	echo "checksum_error";
}
?>
