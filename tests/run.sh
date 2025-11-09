#!/bin/bash

set -eu

RED='\033[1;31m'
GREEN='\033[1;32m'
YELLOW='\033[1;33m'
BLUE='\033[1;34m'
RESET='\033[0m'

log() {
    echo -e "${BLUE}[LOG]${RESET} $1"
}

diff_with_details() {
  local expected_file=$1
  local actual_file=$2
  local prefix=$3

  if diff -u "$expected_file" "$actual_file" > /tmp/diff_output.txt ; then
    return 0
  else
    printf "${RED}${prefix}Ожидалось:${RESET}\n"
    cat "$expected_file" | sed 's/^/  /'
    printf "${RED}${prefix}Получено:${RESET}\n"
    cat "$actual_file" | sed 's/^/  /'
    printf "${RED}${prefix}Подробности различий (diff -u):${RESET}\n"
    sed 's/^/  /' /tmp/diff_output.txt
    return 1
  fi
}

compare_expected_files() {
  local test_dir=$1
  local expected_files_txt="$test_dir/expected.files.txt"
  local failed=false

  [[ -f "$expected_files_txt" ]] || return 0

  log "Проверка expected.files.txt в $test_dir"

  local current_file=""
  local current_content=""
  local temp_expected=$(mktemp)
  local temp_actual=$(mktemp)

  while IFS= read -r line || [[ -n "$line" ]]; do
    if [[ -z "$current_file" ]]; then
      current_file="$line"
      file_path="$current_file"
      current_content=""
      log "Ожидается файл: $current_file"
    elif [[ "$line" == "===EOF===" ]]; then
      echo -e "$current_content" > "$temp_expected"
      if [[ ! -f "$file_path" ]]; then
        printf "${RED}Ожидаемый файл '$file_path' не найден${RESET}\n"
        failed=true
      else
        cp "$file_path" "$temp_actual"
        if ! diff_with_details "$temp_expected" "$temp_actual" "Файл '$current_file': "; then
          failed=true
        fi
      fi
      current_file=""
      current_content=""
    else
      if [[ -z "$current_content" ]]; then
        current_content="$line"
      else
        current_content="$current_content\n$line"
      fi
    fi
  done < "$expected_files_txt"

  rm -f "$temp_expected" "$temp_actual"

  $failed && return 1 || return 0
}

run_test_case() {
  local test_dir=$1
  local run_cmd_file="$test_dir/run.txt"
  local run_before_file="$test_dir/run_before.txt"
  local run_after_file="$test_dir/run_after.txt"
  local input_file="$test_dir/input.txt"
  local expected_output_file="$test_dir/expected.output.txt"

  log "Начинаем тест: $test_dir"

  if [[ ! -f "$run_cmd_file" ]]; then
    printf "${YELLOW}Пропущен run.txt в $test_dir${RESET}\n"
    return 1
  fi

  if [[ -f "$run_before_file" ]]; then
    log "Выполняем run_before.txt"
    if ! sh -c "$(cat "$run_before_file")"; then
      printf "${RED}Ошибка при выполнении run_before.txt в $test_dir${RESET}\n"
      return 1
    fi
  fi

  local run_cmd
  run_cmd=$(cat "$run_cmd_file")
  log "Выполняем команду: $run_cmd"

  local output_file
  output_file=$(mktemp)

  if [[ -f "$input_file" ]]; then
    log "Используем input из $input_file"
    timeout 30s sh -c "$run_cmd" < "$input_file" > "$output_file" 2>&1
  else
    timeout 30s sh -c "$run_cmd" > "$output_file" 2>&1
  fi

  local cmd_exit_code=$?
  
  if [[ $cmd_exit_code -eq 124 ]]; then
    printf "${RED}Таймаут выполнения команды в $test_dir${RESET}\n"
    cat "$output_file"
    rm -f "$output_file"
    return 1
  elif [[ $cmd_exit_code -ne 0 ]]; then
    printf "${RED}Команда завершилась с ошибкой $cmd_exit_code в $test_dir${RESET}\n"
    cat "$output_file"
    rm -f "$output_file"
    return 1
  fi

  local passed=true

  if [[ -f "$expected_output_file" ]]; then
    log "Проверяем вывод..."
    if ! diff_with_details "$expected_output_file" "$output_file" "Вывод: "; then
      passed=false
    fi
  else
    log "Файл expected.output.txt не найден, пропускаем проверку вывода"
  fi

  if ! compare_expected_files "$test_dir"; then
    passed=false
  fi

  rm -f "$output_file"

  if $passed && [[ -f "$run_after_file" ]]; then
    log "Выполняем run_after.txt"
    if ! sh -c "$(cat "$run_after_file")"; then
      printf "${RED}Ошибка при выполнении run_after.txt в $test_dir${RESET}\n"
      return 1
    fi
  fi

  if $passed; then
    log "Тест $test_dir пройден"
    return 0
  else
    log "Тест $test_dir не пройден"
    return 1
  fi
}

run_all_tests() {
  local base_dir=$1
  local passed=0
  local failed=0

  log "Начинаем тестирование в каталоге: $base_dir"

  if [[ ! -d "$base_dir" ]]; then
    printf "${RED}Каталог $base_dir не найден${RESET}\n"
    exit 1
  fi

  local test_dirs=("$base_dir"/*/)
  if [[ ${#test_dirs[@]} -eq 0 ]]; then
    printf "${RED}В каталоге $base_dir не найдено подкаталогов с тестами${RESET}\n"
    exit 1
  fi

  for test_dir in "${test_dirs[@]}"; do
    [[ -d "$test_dir" ]] || continue
    printf "Запускаем тест: $test_dir\n"
    
    if run_test_case "$test_dir"; then
      printf "${GREEN}Тест $test_dir: ПРОЙДЕН${RESET}\n"
      passed=$((passed + 1))
    else
      printf "${RED}Тест $test_dir: НЕ ПРОЙДЕН${RESET}\n"
      failed=$((failed + 1))
    fi
    echo "----------------------------------------"
  done

  printf "\n"
  printf "ИТОГИ ПРОВЕРКИ:\n"
  printf "${GREEN}Успешно: $passed${RESET}, ${RED}Неудачи: $failed${RESET}\n"

  if [[ "$failed" -gt 0 ]]; then
    exit 1
  else
    exit 0
  fi
}

if [[ "$#" -ne 1 ]]; then
  printf "${YELLOW}Использование: %s <каталог_тестов>${RESET}\n" "$0"
  exit 1
fi

run_all_tests "$1"