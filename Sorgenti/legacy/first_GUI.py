import pygame
import sys
import subprocess
import os

# Inizializza Pygame
pygame.init()

# Ottieni le dimensioni dello schermo
screen_info = pygame.display.Info()
screen_width = screen_info.current_w
screen_height = screen_info.current_h

# Imposta le dimensioni della finestra
screen = pygame.display.set_mode((screen_width, screen_height))
pygame.display.set_caption("Esploriamo il Corpo Umano")

# Colori
LIGHT_YELLOW = (255, 255, 224)
BLACK = (0, 0, 0)
DARK_BLUE = (0, 0, 255)
LIGHT_BLUE = (173, 216, 230)
DARK_GREEN = (0, 100, 0)
LIGHT_GREEN = (144, 238, 144)
DARK_RED = (255, 0, 0)
LIGHT_RED = (255, 182, 193)
ORANGE = (255, 165, 0)
WHITE = (255, 255, 255)

# Font
title_font = pygame.font.Font(None, 100)
button_font = pygame.font.Font(None, 48)

# Testo
title_text = title_font.render("LET'S EXPLORE THE HUMAN BODY", True, ORANGE)
start_text = button_font.render("Play", True, DARK_GREEN)
instructions_text = button_font.render("Instructions", True, DARK_BLUE)
exit_text = button_font.render("Exit", True, DARK_RED)

# Posizioni del testo
title_rect = title_text.get_rect(center=(screen_width // 2, screen_height // 4))
start_rect = pygame.Rect(screen_width // 2 - 100, screen_height // 2 - 75, 220, 60)
instructions_rect = pygame.Rect(screen_width // 2 - 100, screen_height // 2, 220, 60)
exit_rect = pygame.Rect(screen_width // 2 - 100, screen_height // 2 + 75, 220, 60)

# Funzione per aprire la finestra delle istruzioni
def start_instructions():
    script_path = os.path.join(os.path.dirname(__file__), "instructions.py")
    if os.path.exists(script_path):
        # Apri instructions.py in un nuovo processo
        subprocess.Popen([sys.executable, script_path])
    else:
        print(f"File {script_path} non trovato.")

# Funzione per avviare il gioco
def start_game():
    script_path = os.path.join(os.path.dirname(__file__), "principal_GUI.py")
    if os.path.exists(script_path):
        subprocess.Popen([sys.executable, script_path])
    else:
        print(f"File {script_path} non trovato.")

# Loop principale
running = True
while running:
    screen.fill(LIGHT_YELLOW)  # Sfondo giallo chiaro
    screen.blit(title_text, title_rect)

    # Disegna i bottoni con colori più chiari e contorno spesso
    pygame.draw.rect(screen, LIGHT_GREEN, start_rect, border_radius=25)
    pygame.draw.rect(screen, DARK_GREEN, start_rect, 5, border_radius=25)  # Contorno spesso
    pygame.draw.rect(screen, LIGHT_BLUE, instructions_rect, border_radius=25)
    pygame.draw.rect(screen, DARK_BLUE, instructions_rect, 5, border_radius=25)  # Contorno spesso
    pygame.draw.rect(screen, LIGHT_RED, exit_rect, border_radius=25)
    pygame.draw.rect(screen, DARK_RED, exit_rect, 5, border_radius=25)  # Contorno spesso

    # Disegna il testo sui bottoni
    screen.blit(start_text, start_text.get_rect(center=start_rect.center))
    screen.blit(instructions_text, instructions_text.get_rect(center=instructions_rect.center))
    screen.blit(exit_text, exit_text.get_rect(center=exit_rect.center))

    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False
        elif event.type == pygame.MOUSEBUTTONDOWN:
            if start_rect.collidepoint(event.pos):
                start_game()
            elif instructions_rect.collidepoint(event.pos):
                start_instructions()  # Apri la finestra delle istruzioni
            elif exit_rect.collidepoint(event.pos):
                running = False

    pygame.display.flip()

pygame.quit()
sys.exit()
