Set-ExecutionPolicy RemoteSigned -scope CurrentUser;
invoke-expression 'powershell -Command { iwr -useb get.scoop.sh | iex; }'
scoop install git;