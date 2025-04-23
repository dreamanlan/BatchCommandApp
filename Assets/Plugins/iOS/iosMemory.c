#import <os/proc.h>
#import <mach/mach_time.h>
#import <mach/mach.h>
#import <mach/mach_host.h>
#import <mach/task_info.h>
#import <mach/task.h>

float ios_GetTotalPhysicalMemory()
{
    kern_return_t kr;
    mach_msg_type_number_t info_count = TASK_VM_INFO_COUNT;
    task_vm_info_data_t vm_info;
    kr = task_info(mach_task_self(), TASK_VM_INFO, (task_info_t)&vm_info, &info_count);
    if (kr == KERN_SUCCESS) return (float)(vm_info.phys_footprint) / 1024.0 / 1024.0;
    return 0;
}

float ios_GetTotalPhysicalMemoryV2()
{
    kern_return_t kr;
    mach_msg_type_number_t info_count = TASK_VM_INFO_COUNT;
    task_vm_info_data_t vm_info;
    kr = task_info(mach_task_self(), TASK_VM_INFO, (task_info_t)&vm_info, &info_count);
    if (kr == KERN_SUCCESS) return (float)(vm_info.resident_size+vm_info.compressed) / 1024.0 / 1024.0;
    return 0;
}

long long ios_GetOsProcAvailableMemory()
{
	size_t mem = os_proc_available_memory();
	return (long long)mem;
}

float ios_GetAppMemory(void) {
    task_vm_info_data_t vmInfo;
    mach_msg_type_number_t count = TASK_VM_INFO_COUNT;
    kern_return_t kernelReturn = task_info(mach_task_self(), TASK_VM_INFO, (task_info_t) &vmInfo, &count);
    if( kernelReturn == KERN_SUCCESS ) {
      return vmInfo.phys_footprint/1024.0f/1024.0f;
    } else {
      return 0.0f;
    }
}
